import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  OnInit,
  QueryList,
  ViewChildren,
  inject
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTableModule } from '@angular/material/table';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
import { AuthService } from '../../core/auth/auth.service';
import { apiErrorMessage } from '../../core/http/api-error.util';
import { InventaireApiService } from '../../core/services/inventaire-api.service';
import { SettingsPreferencesService } from '../../core/services/settings-preferences.service';
import { KpiApiService } from '../../core/services/kpi-api.service';
import { Inventaire } from '../../shared/models/inventaire.model';
import { KpiDashboard, KpiFilter } from '../../shared/models/kpi.model';

Chart.register(...registerables);

const REFRESH_INTERVAL_MS = 30_000;

@Component({
  selector: 'app-kpi-dashboard',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatTableModule
  ],
  templateUrl: './kpi-dashboard.component.html',
  styleUrl: './kpi-dashboard.component.scss'
})
export class KpiDashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly kpiApi = inject(KpiApiService);
  private readonly inventaireApi = inject(InventaireApiService);
  private readonly prefs = inject(SettingsPreferencesService);

  readonly user = this.auth.getCurrentUser();
  readonly clientColumns = ['zone', 'inventaires', 'fiabilite', 'ecarts', 'temps'];

  @ViewChildren('chartCanvas') chartCanvases!: QueryList<ElementRef<HTMLCanvasElement>>;

  filter: KpiFilter = { inventaireId: null };
  dashboard: KpiDashboard | null = null;
  inventaires: Inventaire[] = [];
  loading = true;
  error: string | null = null;
  showFeasibility = false;
  autoRefresh = true;

  private charts: Chart[] = [];
  private viewReady = false;
  private refreshTimer: ReturnType<typeof setInterval> | null = null;

  ngOnInit(): void {
    const preferred = this.prefs.load().inventaireDefautId;
    if (preferred != null) {
      this.filter = { inventaireId: preferred };
    }

    this.inventaireApi.getInventaires(1, 100).subscribe({
      next: (res) => {
        this.inventaires = res.items;
        if (
          this.filter.inventaireId != null &&
          !res.items.some((i) => i.id === this.filter.inventaireId)
        ) {
          this.filter = { inventaireId: null };
          this.loadDashboard(false);
        }
      }
    });
    this.loadDashboard(false);
    this.startAutoRefresh();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.chartCanvases.changes.subscribe(() => this.renderCharts());
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
    this.destroyCharts();
  }

  @HostListener('window:resize')
  onResize(): void {
    this.charts.forEach((c) => c.resize());
  }

  applyFilters(): void {
    this.loadDashboard(true);
  }

  resetFilters(): void {
    this.filter = { inventaireId: null };
    this.loadDashboard(true);
  }

  compareInventaire(a: number | null | undefined, b: number | null | undefined): boolean {
    return a === b || (a == null && b == null);
  }

  toggleAutoRefresh(enabled: boolean): void {
    this.autoRefresh = enabled;
    if (enabled) {
      this.startAutoRefresh();
    } else {
      this.stopAutoRefresh();
    }
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }

  feasibilityClass(status: string): string {
    return status.toLowerCase();
  }

  private startAutoRefresh(): void {
    this.stopAutoRefresh();
    if (!this.autoRefresh) return;
    this.refreshTimer = setInterval(() => this.loadDashboard(false), REFRESH_INTERVAL_MS);
  }

  private stopAutoRefresh(): void {
    if (this.refreshTimer) {
      clearInterval(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  private loadDashboard(showLoader: boolean): void {
    if (showLoader) {
      this.loading = true;
      this.error = null;
      this.destroyCharts();
    }

    this.kpiApi.getDashboard(this.filter).subscribe({
      next: (data) => {
        this.dashboard = data;
        this.loading = false;
        setTimeout(() => this.renderCharts());
      },
      error: (err) => {
        this.error = apiErrorMessage(err, 'Impossible de charger les KPI.');
        this.loading = false;
      }
    });
  }

  private renderCharts(): void {
    if (!this.viewReady || !this.dashboard) return;
    this.destroyCharts();

    const d = this.dashboard;
    const chartOpts = { responsive: true, maintainAspectRatio: false };

    this.createChart('ecartsType', {
      type: 'doughnut',
      data: {
        labels: this.withFallback(d.precision.ecartsParType.map((x) => x.label), ['MANQUANT', 'SURPLUS']),
        datasets: [{
          data: this.withFallbackValues(d.precision.ecartsParType, ['MANQUANT', 'SURPLUS']),
          backgroundColor: ['#ef4444', '#f59e0b']
        }]
      },
      options: { ...chartOpts, plugins: { legend: { position: 'bottom' } } }
    });

    this.createChart('ecartsJour', {
      type: 'line',
      data: {
        labels: d.precision.ecartsParJour.map((x) => x.label),
        datasets: [{
          label: 'Écarts',
          data: d.precision.ecartsParJour.map((x) => x.value),
          borderColor: '#5b66f5',
          backgroundColor: 'rgba(91,102,245,0.1)',
          tension: 0.3,
          fill: true
        }]
      },
      options: { ...chartOpts, scales: { y: { beginAtZero: true } } }
    });

    this.createChart('avancement', {
      type: 'bar',
      data: {
        labels: d.performance.avancementParInventaire.map((x) => x.label),
        datasets: [{
          label: 'Avancement %',
          data: d.performance.avancementParInventaire.map((x) => x.value),
          backgroundColor: '#22c55e'
        }]
      },
      options: { ...chartOpts, scales: { y: { max: 100, beginAtZero: true } } }
    });

    this.createChart('productivite', {
      type: 'bar',
      data: {
        labels: d.performance.productiviteOperateurs.map((x) => x.nom),
        datasets: [
          {
            label: 'US/h',
            data: d.performance.productiviteOperateurs.map((x) => x.unitesParHeure),
            backgroundColor: '#5b66f5'
          },
          {
            label: 'Empl./h',
            data: d.performance.productiviteOperateurs.map((x) => x.emplacementsParHeure),
            backgroundColor: '#06b6d4'
          }
        ]
      },
      options: { ...chartOpts, scales: { y: { beginAtZero: true } } }
    });

    this.createChart('qualite', {
      type: 'bar',
      data: {
        labels: d.qualite.indicateurs?.map((x) => x.label) ?? [],
        datasets: [{
          label: 'Valeur',
          data: d.qualite.indicateurs?.map((x) => x.value) ?? [],
          backgroundColor: ['#10b981', '#3b82f6', '#f59e0b', '#8b5cf6', '#ef4444']
        }]
      },
      options: { ...chartOpts, scales: { y: { beginAtZero: true } } }
    });

    this.createGauge('gaugeValidation', d.qualite.tauxValidationPremierComptage, '#10b981');
    this.createGauge('gaugeConformite', d.qualite.tauxConformiteEmplacements, '#3b82f6');
    this.createGauge('gaugeOccupation', d.wms.tauxOccupationEntrepot, '#8b5cf6');

    this.createChart('usJour', {
      type: 'line',
      data: {
        labels: d.wms.unitesInventorieesParJour.map((x) => x.label),
        datasets: [{
          label: 'US inventoriées (scans)',
          data: d.wms.unitesInventorieesParJour.map((x) => x.value),
          borderColor: '#8b5cf6',
          tension: 0.3,
          fill: false
        }]
      },
      options: { ...chartOpts, scales: { y: { beginAtZero: true } } }
    });

    this.createChart('emplJour', {
      type: 'bar',
      data: {
        labels: d.wms.emplacementsInventoriesParJour.map((x) => x.label),
        datasets: [{
          label: 'Emplacements terminés',
          data: d.wms.emplacementsInventoriesParJour.map((x) => x.value),
          backgroundColor: '#06b6d4'
        }]
      },
      options: { ...chartOpts, scales: { y: { beginAtZero: true } } }
    });

    this.createChart('clients', {
      type: 'bar',
      data: {
        labels: d.clients.parZone.map((x) => x.zone),
        datasets: [
          {
            label: 'Fiabilité %',
            data: d.clients.parZone.map((x) => x.tauxFiabiliteStock),
            backgroundColor: '#10b981'
          },
          {
            label: 'Écarts (unités)',
            data: d.clients.parZone.map((x) => x.valeurEcartsUnites),
            backgroundColor: '#f97316'
          }
        ]
      },
      options: { ...chartOpts, scales: { y: { beginAtZero: true } } }
    });

    this.createChart('delai', {
      type: 'doughnut',
      data: {
        labels: ['Délai moyen (h)', 'Reste'],
        datasets: [{
          data: [
            d.wms.delaiMoyenDetectionCorrectionHeures,
            Math.max(24 - d.wms.delaiMoyenDetectionCorrectionHeures, 0)
          ],
          backgroundColor: ['#ef4444', '#e5e7eb']
        }]
      },
      options: { ...chartOpts, plugins: { legend: { position: 'bottom' } } }
    });
  }

  private withFallback(labels: string[], fallback: string[]): string[] {
    return labels.length > 0 ? labels : fallback;
  }

  private withFallbackValues(
    points: { label: string; value: number }[],
    keys: string[]
  ): number[] {
    if (points.length === 0) return keys.map(() => 0);
    return points.map((x) => x.value);
  }

  private createGauge(key: string, value: number, color: string): void {
    const pct = Math.min(Math.max(value, 0), 100);
    const config: ChartConfiguration<'doughnut'> = {
      type: 'doughnut',
      data: {
        labels: ['Atteint', 'Reste'],
        datasets: [{
          data: [pct, 100 - pct],
          backgroundColor: [color, '#e5e7eb'],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        circumference: 180,
        rotation: 270,
        cutout: '70%',
        plugins: { legend: { display: false } }
      }
    };
    this.createChart(key, config);
  }

  private createChart(key: string, config: ChartConfiguration): void {
    const canvas = this.chartCanvases.find((c) => c.nativeElement.dataset['chart'] === key);
    if (!canvas) return;
    this.charts.push(new Chart(canvas.nativeElement, config));
  }

  private destroyCharts(): void {
    this.charts.forEach((c) => c.destroy());
    this.charts = [];
  }
}
