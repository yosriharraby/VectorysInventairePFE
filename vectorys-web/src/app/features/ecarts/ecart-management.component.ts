import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/auth/auth.service';
import { apiErrorMessage } from '../../core/http/api-error.util';
import { EcartApiService } from '../../core/services/ecart-api.service';
import { InventaireApiService } from '../../core/services/inventaire-api.service';
import { SettingsPreferencesService } from '../../core/services/settings-preferences.service';
import {
  AssignationRecomptageMode,
  BrancheBMode,
  Ecart,
  EcartsList,
  Recomptage
} from '../../shared/models/ecart.model';
import { Inventaire } from '../../shared/models/inventaire.model';
import { Agent, agentDisplayName } from '../../shared/models/agent.model';

@Component({
  selector: 'app-ecart-management',
  standalone: true,
  imports: [
    RouterLink,
    DatePipe,
    FormsModule,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatIconModule,
    MatRadioModule,
    MatSelectModule,
    MatMenuModule,
    MatTableModule
  ],
  templateUrl: './ecart-management.component.html',
  styleUrl: './ecart-management.component.scss'
})
export class EcartManagementComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly inventaireApi = inject(InventaireApiService);
  private readonly ecartApi = inject(EcartApiService);
  private readonly prefs = inject(SettingsPreferencesService);

  readonly user = this.auth.getCurrentUser();
  readonly agentDisplayName = agentDisplayName;
  inventaires: Inventaire[] = [];
  selectedInventaireId: number | null = null;
  ecarts: EcartsList | null = null;
  selectedIds = new Set<number>();
  loading = true;
  detecting = false;
  applying = false;
  applyingB = false;
  error: string | null = null;
  success: string | null = null;

  brancheBStep: 0 | 1 | 2 = 0;
  brancheBMode: BrancheBMode = 'GLOBAL';
  brancheBFile: File | null = null;
  assignationMode: AssignationRecomptageMode = 'MEME_AGENT';
  selectedAgent: Agent | null = null;
  agents: Agent[] = [];
  existingRecomptages: Recomptage[] = [];
  loadingRecomptages = false;
  exportingC1 = false;

  displayedColumns = ['select', 'emplacement', 'codeUS', 'client', 'qteTheorique', 'qteScannee', 'delta', 'type'];

  ngOnInit(): void {
    const openBrancheB = this.route.snapshot.queryParamMap.get('openBrancheB');
    const inventaireIdParam = this.route.snapshot.queryParamMap.get('inventaireId');

    this.inventaireApi.getInventaires(1, 50).subscribe({
      next: (res) => {
        this.inventaires = res.items;
        const queryId = inventaireIdParam ? Number(inventaireIdParam) : null;
        const defaultId = queryId && !Number.isNaN(queryId)
          ? queryId
          : this.prefs.resolveInventaireId(res.items.map((i) => i.id));
        if (defaultId != null) {
          void this.selectInventaire(defaultId);
          if (openBrancheB === '1') {
            this.openBrancheBModal();
          }
        } else {
          this.loading = false;
        }
      },
      error: (err) => {
        this.error = apiErrorMessage(err, 'Impossible de charger les inventaires.');
        this.loading = false;
      }
    });
  }

  selectInventaire(id: number, successMessage?: string | null): void {
    this.selectedInventaireId = id;
    this.loading = true;
    this.error = null;
    this.success = successMessage ?? null;
    this.selectedIds.clear();

    this.ecartApi.getByInventaire(id).subscribe({
      next: (res) => {
        this.ecarts = res;
        this.loading = false;
      },
      error: () => {
        this.error = 'Impossible de charger les écarts.';
        this.loading = false;
      }
    });
  }

  detecter(): void {
    if (!this.selectedInventaireId || !this.canDetecter) return;
    this.detecting = true;
    this.error = null;
    this.success = null;
    this.selectedIds.clear();

    this.ecartApi.detecter(this.selectedInventaireId).subscribe({
      next: (res) => {
        this.detecting = false;
        this.selectInventaire(this.selectedInventaireId!, res.message ?? `${res.ecartsDetectes} écart(s) détecté(s).`);
      },
      error: (err) => {
        this.detecting = false;
        this.error = err?.error?.message ?? 'Échec de la détection des écarts.';
      }
    });
  }

  appliquerBrancheA(): void {
    if (!this.selectedInventaireId || !this.canAppliquerBrancheA) return;
    const ids = this.selectedIds.size > 0
      ? Array.from(this.selectedIds)
      : this.pendingEcarts.map((e) => e.id);

    if (ids.length === 0) {
      this.error = 'Aucun écart en attente à traiter.';
      return;
    }

    this.applying = true;
    this.error = null;
    this.success = null;

    this.ecartApi.appliquerBrancheA({ inventaireId: this.selectedInventaireId, ecartIds: ids }).subscribe({
      next: (res) => {
        this.applying = false;
        this.success = res.message;
        this.selectInventaire(this.selectedInventaireId!, res.message);
      },
      error: (err) => {
        this.applying = false;
        this.error = err?.error?.message ?? 'Échec de l\'application Branche A.';
      }
    });
  }

  get pendingEcarts(): Ecart[] {
    return this.ecarts?.items ?? [];
  }

  get canDetecter(): boolean {
    if (!this.selectedInventaireId || this.detecting) return false;
    return !(this.ecarts?.tousRecomptagesCorriges && this.pendingEcarts.length === 0);
  }

  get canAppliquerBrancheA(): boolean {
    return !this.applying && this.pendingEcarts.length > 0;
  }

  get allPendingSelected(): boolean {
    const pending = this.pendingEcarts;
    return pending.length > 0 && pending.every((e) => this.selectedIds.has(e.id));
  }

  toggleAll(checked: boolean): void {
    if (checked) {
      this.pendingEcarts.forEach((e) => this.selectedIds.add(e.id));
    } else {
      this.selectedIds.clear();
    }
  }

  toggleOne(id: number, checked: boolean): void {
    if (checked) {
      this.selectedIds.add(id);
    } else {
      this.selectedIds.delete(id);
    }
  }

  isSelected(id: number): boolean {
    return this.selectedIds.has(id);
  }

  openBrancheBModal(): void {
    if (!this.selectedInventaireId) return;
    this.brancheBMode = 'GLOBAL';
    this.brancheBFile = null;
    this.assignationMode = 'MEME_AGENT';
    this.selectedAgent = null;
    this.existingRecomptages = [];
    this.brancheBStep = 1;
    this.error = null;
  }

  closeBrancheBModal(): void {
    if (this.applyingB) return;
    this.brancheBStep = 0;
    this.brancheBFile = null;
  }

  continueBrancheB(): void {
    if (!this.selectedInventaireId) return;
    this.brancheBStep = 2;
    this.error = null;
    this.assignationMode = this.brancheBMode === 'GLOBAL' ? 'EXCEL' : 'MEME_AGENT';
    this.selectedAgent = null;

    this.inventaireApi.getAgents().subscribe({
      next: (agents) => {
        this.agents = agents;
      }
    });

    this.loadingRecomptages = true;
    this.ecartApi.getRecomptages(this.selectedInventaireId).subscribe({
      next: (res) => {
        this.existingRecomptages = res.items;
        this.loadingRecomptages = false;
      },
      error: () => {
        this.loadingRecomptages = false;
      }
    });
  }

  backToBrancheBChoice(): void {
    if (this.applyingB) return;
    this.brancheBStep = 1;
    this.brancheBFile = null;
  }

  onBrancheBFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.brancheBFile = input.files?.[0] ?? null;
  }

  exportC1Stock(): void {
    if (!this.selectedInventaireId) return;
    this.exportingC1 = true;
    this.ecartApi.exportC1Stock(this.selectedInventaireId).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `inventaire-${this.selectedInventaireId}-c1-stock.xlsx`;
        a.click();
        URL.revokeObjectURL(url);
        this.exportingC1 = false;
      },
      error: () => {
        this.exportingC1 = false;
        this.error = 'Échec de l\'export du stock C1.';
      }
    });
  }

  confirmBrancheB(): void {
    if (!this.selectedInventaireId) return;
    if (!this.brancheBFile) {
      this.error = 'Veuillez sélectionner un fichier Excel filtré.';
      return;
    }

    if (this.assignationMode === 'NOUVEAU_AGENT' && !this.selectedAgent) {
      this.error = 'Veuillez sélectionner un agent.';
      return;
    }

    this.applyingB = true;
    this.error = null;
    this.success = null;

    const agentCode = this.assignationMode === 'NOUVEAU_AGENT' ? this.selectedAgent?.codePersonnel : undefined;

    this.ecartApi.appliquerBrancheB(
      this.selectedInventaireId,
      this.brancheBMode,
      this.brancheBFile,
      this.assignationMode,
      agentCode
    ).subscribe({
      next: (res) => {
        this.applyingB = false;
        this.brancheBStep = 0;
        this.brancheBFile = null;
        void this.router.navigate(['/ecarts/recomptages', this.selectedInventaireId], {
          queryParams: { success: res.message }
        });
      },
      error: (err) => {
        this.applyingB = false;
        this.error = apiErrorMessage(err, 'Échec du lancement du recomptage.');
      }
    });
  }

  goToRecomptages(): void {
    if (!this.selectedInventaireId) return;
    void this.router.navigate(['/ecarts/recomptages', this.selectedInventaireId]);
  }

  get unselectedAgents(): Agent[] {
    const selectedId = this.selectedAgent?.id;
    return this.agents.filter((a) => a.id !== selectedId);
  }

  pickRecomptageAgent(agent: Agent): void {
    this.selectedAgent = agent;
  }

  clearRecomptageAgent(): void {
    this.selectedAgent = null;
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
