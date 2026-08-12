import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  KpiClients,
  KpiDashboard,
  KpiFeasibilityNote,
  KpiFilter,
  KpiPerformance,
  KpiPrecision,
  KpiQualite,
  KpiWms
} from '../../shared/models/kpi.model';

@Injectable({ providedIn: 'root' })
export class KpiApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/kpi`;

  getDashboard(filter: KpiFilter = {}): Observable<KpiDashboard> {
    return this.http.get<KpiDashboard>(`${this.base}/dashboard`, { params: this.toParams(filter) });
  }

  getPrecision(filter: KpiFilter = {}): Observable<KpiPrecision> {
    return this.http.get<KpiPrecision>(`${this.base}/precision`, { params: this.toParams(filter) });
  }

  getPerformance(filter: KpiFilter = {}): Observable<KpiPerformance> {
    return this.http.get<KpiPerformance>(`${this.base}/performance`, { params: this.toParams(filter) });
  }

  getQualite(filter: KpiFilter = {}): Observable<KpiQualite> {
    return this.http.get<KpiQualite>(`${this.base}/qualite`, { params: this.toParams(filter) });
  }

  getClients(filter: KpiFilter = {}): Observable<KpiClients> {
    return this.http.get<KpiClients>(`${this.base}/clients`, { params: this.toParams(filter) });
  }

  getWms(filter: KpiFilter = {}): Observable<KpiWms> {
    return this.http.get<KpiWms>(`${this.base}/wms`, { params: this.toParams(filter) });
  }

  getFeasibility(): Observable<KpiFeasibilityNote[]> {
    return this.http.get<KpiFeasibilityNote[]>(`${this.base}/feasibility`);
  }

  private toParams(filter: KpiFilter): HttpParams {
    let params = new HttpParams();
    if (filter.dateFrom) params = params.set('dateFrom', filter.dateFrom);
    if (filter.dateTo) params = params.set('dateTo', filter.dateTo);
    if (filter.inventaireId != null && filter.inventaireId > 0) {
      params = params.set('inventaireId', filter.inventaireId);
    }
    if (filter.zone) params = params.set('zone', filter.zone);
    return params;
  }
}
