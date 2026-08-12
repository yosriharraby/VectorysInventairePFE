import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AppliquerBrancheARequest,
  AppliquerBrancheAResult,
  AppliquerBrancheBRequest,
  AppliquerBrancheBResult,
  DetecterEcartsResult,
  EcartsCount,
  EcartsList
} from '../../shared/models/ecart.model';

@Injectable({ providedIn: 'root' })
export class EcartApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/ecarts`;

  getByInventaire(inventaireId: number): Observable<EcartsList> {
    return this.http.get<EcartsList>(this.base, { params: { inventaireId } });
  }

  getCount(): Observable<EcartsCount> {
    return this.http.get<EcartsCount>(`${this.base}/count`);
  }

  detecter(inventaireId: number): Observable<DetecterEcartsResult> {
    return this.http.post<DetecterEcartsResult>(`${this.base}/detecter/${inventaireId}`, {});
  }

  appliquerBrancheA(body: AppliquerBrancheARequest): Observable<AppliquerBrancheAResult> {
    return this.http.post<AppliquerBrancheAResult>(`${this.base}/branche-a`, body);
  }

  appliquerBrancheB(body: AppliquerBrancheBRequest): Observable<AppliquerBrancheBResult> {
    return this.http.post<AppliquerBrancheBResult>(`${this.base}/branche-b`, body);
  }
}
