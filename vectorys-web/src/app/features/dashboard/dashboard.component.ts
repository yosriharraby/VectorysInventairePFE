import { DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { AuthService } from '../../core/auth/auth.service';
import { apiErrorMessage } from '../../core/http/api-error.util';
import { EcartApiService } from '../../core/services/ecart-api.service';
import { InventaireApiService } from '../../core/services/inventaire-api.service';
import { KpiApiService } from '../../core/services/kpi-api.service';
import { Inventaire } from '../../shared/models/inventaire.model';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe, RouterLink, MatButtonModule, MatIconModule, MatTableModule, StatusBadgeComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly inventaireApi = inject(InventaireApiService);
  private readonly ecartApi = inject(EcartApiService);
  private readonly kpiApi = inject(KpiApiService);

  readonly user = this.auth.getCurrentUser();
  displayedColumns = ['reference', 'zone', 'dateDebut', 'agents', 'conformite', 'statut', 'action'];
  inventaires: Inventaire[] = [];
  totalInventaires = 0;
  totalEcarts = 0;
  tauxConformite: number | null = null;
  loading = true;
  error: string | null = null;
  success: string | null = null;
  busyId: number | null = null;

  ngOnInit(): void {
    this.inventaireApi.getInventaires().subscribe({
      next: (res) => {
        this.inventaires = res.items;
        this.totalInventaires = res.totalCount;
        this.loading = false;
      },
      error: (err) => {
        this.error = apiErrorMessage(err, 'Impossible de charger les inventaires.');
        this.loading = false;
      }
    });

    this.ecartApi.getCount().subscribe({
      next: (res) => {
        this.totalEcarts = res.totalDetectes;
      }
    });

    this.kpiApi.getDashboard().subscribe({
      next: (res) => {
        this.tauxConformite = res.qualite.tauxConformiteEmplacements;
      }
    });
  }

  cloturer(inv: Inventaire): void {
    if (inv.statut === 'CLOTURE') return;
    if (!confirm(`Clôturer l'inventaire ${inv.reference} ?`)) return;
    this.busyId = inv.id;
    this.error = null;
    this.success = null;
    this.inventaireApi.cloturerInventaire(inv.id).subscribe({
      next: (res) => {
        this.busyId = null;
        this.success = res.message;
        inv.statut = res.inventaire.statut;
        inv.dateFin = res.inventaire.dateFin;
      },
      error: (err) => {
        this.busyId = null;
        this.error = apiErrorMessage(err, 'Échec de la clôture.');
      }
    });
  }

  exporter(inv: Inventaire): void {
    this.busyId = inv.id;
    this.error = null;
    this.inventaireApi.exportInventaire(inv.id).subscribe({
      next: (blob) => {
        this.busyId = null;
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Inventaire_${inv.reference}.xlsx`;
        a.click();
        URL.revokeObjectURL(url);
      },
      error: (err) => {
        this.busyId = null;
        this.error = apiErrorMessage(err, "Échec de l'export Excel.");
      }
    });
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}

