import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AppliquerBrancheARequest,
  AppliquerBrancheAResult,
  AppliquerBrancheBResult,
  AssignationRecomptageMode,
  BrancheBMode,
  DetecterEcartsResult,
  EcartsCount,
  EcartsList,
  RecomptagesList
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

  appliquerBrancheB(
    inventaireId: number,
    mode: BrancheBMode,
    file: File,
    assignationMode?: AssignationRecomptageMode,
    agentCode?: string
  ): Observable<AppliquerBrancheBResult> {
    const form = new FormData();
    form.append('inventaireId', String(inventaireId));
    form.append('mode', mode);
    form.append('file', file, file.name);
    if (assignationMode) form.append('assignationMode', assignationMode);
    if (agentCode) form.append('agentCode', agentCode);
    return this.http.post<AppliquerBrancheBResult>(`${this.base}/branche-b`, form);
  }

  getRecomptages(inventaireId: number): Observable<RecomptagesList> {
    return this.http.get<RecomptagesList>(`${this.base}/${inventaireId}/recomptages`);
  }

  exportC1Stock(inventaireId: number): Observable<Blob> {
    return this.http.get(`${this.base}/${inventaireId}/export-c1`, { responseType: 'blob' });
  }

  exportRecomptage(recomptageId: number): Observable<Blob> {
    return this.http.get(`${this.base}/recomptages/${recomptageId}/export`, { responseType: 'blob' });
  }
}
