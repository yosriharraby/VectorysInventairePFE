import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { Subscription } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { apiErrorMessage } from '../../core/http/api-error.util';
import { InventaireApiService } from '../../core/services/inventaire-api.service';
import { SettingsPreferencesService } from '../../core/services/settings-preferences.service';
import { InventaireHubService } from '../../core/signalr/inventaire-hub.service';
import { Inventaire } from '../../shared/models/inventaire.model';
import { AgentSuivi, SuiviTempsReel } from '../../shared/models/monitoring.model';

@Component({
  selector: 'app-agent-monitoring',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatFormFieldModule, MatIconModule, MatSelectModule, MatTableModule],
  templateUrl: './agent-monitoring.component.html',
  styleUrl: './agent-monitoring.component.scss'
})
export class AgentMonitoringComponent implements OnInit, OnDestroy {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly api = inject(InventaireApiService);
  private readonly hub = inject(InventaireHubService);
  private readonly prefs = inject(SettingsPreferencesService);

  readonly user = this.auth.getCurrentUser();
  inventaires: Inventaire[] = [];
  selectedInventaireId: number | null = null;
  suivi: SuiviTempsReel | null = null;
  loading = true;
  error: string | null = null;
  liveState: 'connected' | 'disconnected' | 'connecting' = 'disconnected';

  displayedColumns = ['agent', 'statut', 'progression', 'emplacements'];

  private readonly subs = new Subscription();

  ngOnInit(): void {
    this.subs.add(
      this.hub.connectionState$.subscribe((state) => {
        this.liveState = state;
      })
    );

    this.subs.add(
      this.hub.nouveauScan$.subscribe((evt) => this.applyNouveauScan(evt))
    );

    this.subs.add(
      this.hub.finComptage$.subscribe((evt) => this.applyFinComptage(evt))
    );

    this.api.getInventaires(1, 50).subscribe({
      next: async (res) => {
        this.inventaires = res.items;
        const defaultId = this.prefs.resolveInventaireId(res.items.map((i) => i.id));
        if (defaultId != null) {
          await this.selectInventaire(defaultId);
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

  ngOnDestroy(): void {
    this.subs.unsubscribe();
    void this.hub.disconnect();
  }

  async selectInventaire(id: number): Promise<void> {
    this.selectedInventaireId = id;
    this.loading = true;
    this.error = null;

    this.api.getSuiviTempsReel(id).subscribe({
      next: async (suivi) => {
        this.suivi = suivi;
        this.loading = false;
        try {
          await this.hub.connect();
          await this.hub.joinInventaireGroup(id);
        } catch {
          this.liveState = 'disconnected';
        }
      },
      error: () => {
        this.error = 'Impossible de charger le suivi temps réel.';
        this.loading = false;
      }
    });
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }

  agentProgress(agent: AgentSuivi): number {
    if (agent.emplacementsTotal === 0) return 0;
    return Math.round((agent.emplacementsTermines / agent.emplacementsTotal) * 100);
  }

  private applyNouveauScan(evt: {
    inventaireId: number;
    agentId: number;
    agentNom: string;
    emplacementId: number;
    emplacementCode: string;
    emplacementDesignation: string;
    qteScannee: number;
    qteAttendue: number;
    emplacementStatut: string;
  }): void {
    if (!this.suivi || evt.inventaireId !== this.suivi.inventaireId) return;

    let agent = this.suivi.agents.find((a) => a.agentId === evt.agentId);
    if (!agent) {
      agent = {
        agentId: evt.agentId,
        nom: evt.agentNom,
        estActif: true,
        emplacementsTermines: 0,
        emplacementsTotal: 0,
        emplacements: []
      };
      this.suivi = { ...this.suivi, agents: [...this.suivi.agents, agent] };
    }

    const agents = this.suivi.agents.map((a) => {
      if (a.agentId !== evt.agentId) return a;
      const emplacements = a.emplacements.map((e) =>
        e.id === evt.emplacementId
          ? {
              ...e,
              qteScannee: evt.qteScannee,
              qteAttendue: evt.qteAttendue,
              statut: evt.emplacementStatut,
              designation: evt.emplacementDesignation || e.designation,
              code: evt.emplacementCode || e.code
            }
          : e
      );
      const termines = emplacements.filter((e) => e.statut === 'TERMINE').length;
      return {
        ...a,
        nom: evt.agentNom,
        estActif: true,
        emplacements,
        emplacementsTermines: termines,
        emplacementsTotal: emplacements.length
      };
    });

    this.suivi = { ...this.suivi, agents };
  }

  private applyFinComptage(evt: {
    agentId: number;
    emplacementId: number;
    statut: string;
    qteScannee: number;
    qteAttendue: number;
  }): void {
    if (!this.suivi) return;

    const agents = this.suivi.agents.map((a) => {
      if (a.agentId !== evt.agentId) return a;
      const emplacements = a.emplacements.map((e) =>
        e.id === evt.emplacementId
          ? { ...e, statut: evt.statut, qteScannee: evt.qteScannee, qteAttendue: evt.qteAttendue }
          : e
      );
      return {
        ...a,
        emplacements,
        emplacementsTermines: emplacements.filter((e) => e.statut === 'TERMINE').length,
        emplacementsTotal: emplacements.length
      };
    });

    this.suivi = { ...this.suivi, agents };
  }
}
