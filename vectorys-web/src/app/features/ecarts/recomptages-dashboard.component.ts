import { DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { AuthService } from '../../core/auth/auth.service';
import { apiErrorMessage } from '../../core/http/api-error.util';
import { EcartApiService } from '../../core/services/ecart-api.service';
import { Recomptage, RecomptagesList } from '../../shared/models/ecart.model';

@Component({
  selector: 'app-recomptages-dashboard',
  standalone: true,
  imports: [RouterLink, DatePipe, MatButtonModule, MatIconModule, MatTableModule],
  templateUrl: './recomptages-dashboard.component.html',
  styleUrl: './recomptages-dashboard.component.scss'
})
export class RecomptagesDashboardComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly ecartApi = inject(EcartApiService);

  readonly user = this.auth.getCurrentUser();
  inventaireId: number | null = null;
  data: RecomptagesList | null = null;
  loading = true;
  error: string | null = null;
  success: string | null = null;

  displayedColumns = ['date', 'mode', 'fichier', 'agents', 'emplacements', 'ecarts', 'statut', 'export'];
  exportingId: number | null = null;

  ngOnInit(): void {
    const success = this.route.snapshot.queryParamMap.get('success');
    if (success) this.success = success;

    const idParam = this.route.snapshot.paramMap.get('inventaireId');
    const id = idParam ? Number(idParam) : NaN;
    if (!id || Number.isNaN(id)) {
      this.error = 'Inventaire invalide.';
      this.loading = false;
      return;
    }

    this.inventaireId = id;
    this.load();
  }

  load(): void {
    if (!this.inventaireId) return;
    this.loading = true;
    this.error = null;

    this.ecartApi.getRecomptages(this.inventaireId).subscribe({
      next: (res) => {
        this.data = res;
        this.loading = false;
      },
      error: (err) => {
        this.error = apiErrorMessage(err, 'Impossible de charger les recomptages.');
        this.loading = false;
      }
    });
  }

  agentsLabel(recomptage: Recomptage): string {
    if (!recomptage.agents?.length) return '—';
    return recomptage.agents
      .map((a) => `${a.codePersonnel} (${a.emplacementsAssignes})`)
      .join(', ');
  }

  modeLabel(mode: string): string {
    return mode === 'GLOBAL' ? 'Intégral' : 'Ciblé';
  }

  statutLabel(statut: string): string {
    if (statut === 'CORRIGE') return 'Corrigé';
    return 'En attente';
  }

  exportRecomptage(row: Recomptage): void {
    this.exportingId = row.id;
    this.ecartApi.exportRecomptage(row.id).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = row.fichierNom
          ? `${row.fichierNom.replace(/\.xlsx?$/i, '')}-export.xlsx`
          : `recomptage-${row.id}.xlsx`;
        a.click();
        URL.revokeObjectURL(url);
        this.exportingId = null;
      },
      error: (err) => {
        this.exportingId = null;
        this.error = apiErrorMessage(err, 'Échec de l\'export du recomptage.');
      }
    });
  }

  nouveauRecomptage(): void {
    void this.router.navigate(['/ecarts'], { queryParams: { inventaireId: this.inventaireId, openBrancheB: 1 } });
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
