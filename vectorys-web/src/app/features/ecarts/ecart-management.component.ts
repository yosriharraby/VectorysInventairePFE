import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { AuthService } from '../../core/auth/auth.service';
import { apiErrorMessage } from '../../core/http/api-error.util';
import { EcartApiService } from '../../core/services/ecart-api.service';
import { InventaireApiService } from '../../core/services/inventaire-api.service';
import { SettingsPreferencesService } from '../../core/services/settings-preferences.service';
import { Ecart, EcartsList } from '../../shared/models/ecart.model';
import { Inventaire } from '../../shared/models/inventaire.model';

@Component({
  selector: 'app-ecart-management',
  standalone: true,
  imports: [
    RouterLink,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatIconModule,
    MatSelectModule,
    MatTableModule
  ],
  templateUrl: './ecart-management.component.html',
  styleUrl: './ecart-management.component.scss'
})
export class EcartManagementComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly inventaireApi = inject(InventaireApiService);
  private readonly ecartApi = inject(EcartApiService);
  private readonly prefs = inject(SettingsPreferencesService);

  readonly user = this.auth.getCurrentUser();
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

  displayedColumns = ['select', 'emplacement', 'codeUS', 'qteTheorique', 'qteScannee', 'delta', 'type', 'statut'];

  ngOnInit(): void {
    this.inventaireApi.getInventaires(1, 50).subscribe({
      next: (res) => {
        this.inventaires = res.items;
        const defaultId = this.prefs.resolveInventaireId(res.items.map((i) => i.id));
        if (defaultId != null) {
          void this.selectInventaire(defaultId);
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

  selectInventaire(id: number): void {
    this.selectedInventaireId = id;
    this.loading = true;
    this.error = null;
    this.success = null;
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
    if (!this.selectedInventaireId) return;
    this.detecting = true;
    this.error = null;
    this.success = null;
    this.selectedIds.clear();

    this.ecartApi.detecter(this.selectedInventaireId).subscribe({
      next: (res) => {
        this.detecting = false;
        this.success = `${res.ecartsDetectes} écart(s) détecté(s).`;
        this.selectInventaire(this.selectedInventaireId!);
      },
      error: (err) => {
        this.detecting = false;
        this.error = err?.error?.message ?? 'Échec de la détection des écarts.';
      }
    });
  }

  appliquerBrancheA(): void {
    if (!this.selectedInventaireId) return;
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
        this.selectInventaire(this.selectedInventaireId!);
      },
      error: (err) => {
        this.applying = false;
        this.error = err?.error?.message ?? 'Échec de l\'application Branche A.';
      }
    });
  }

  get pendingEcarts(): Ecart[] {
    return this.ecarts?.items.filter((e) => e.statut === 'DETECTE') ?? [];
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

  canSelect(ecart: Ecart): boolean {
    return ecart.statut === 'DETECTE';
  }

  appliquerBrancheB(): void {
    if (!this.selectedInventaireId) return;
    if (!confirm('Branche B : recomptage global de TOUS les emplacements C1 vers C2. Continuer ?')) {
      return;
    }
    this.applyingB = true;
    this.error = null;
    this.success = null;
    this.ecartApi.appliquerBrancheB({ inventaireId: this.selectedInventaireId }).subscribe({
      next: (res) => {
        this.applyingB = false;
        this.success = res.message;
        this.selectInventaire(this.selectedInventaireId!);
      },
      error: (err) => {
        this.applyingB = false;
        this.error = err?.error?.message ?? 'Échec de l\'application Branche B.';
      }
    });
  }

  statutLabel(statut: string): string {
    switch (statut) {
      case 'DETECTE': return 'En attente';
      case 'TRAITE_BRANCHE_A': return 'Branche A';
      case 'TRAITE_BRANCHE_B': return 'Branche B';
      default: return statut;
    }
  }
  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}



