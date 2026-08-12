import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Agent } from '../../shared/models/agent.model';
import {
  CloturerInventaireResult,
  CreateInventaireRequest,
  CreateInventaireResult,
  ImportStockResult,
  PagedInventaires,
  PassageDto
} from '../../shared/models/inventaire.model';
import { SuiviTempsReel } from '../../shared/models/monitoring.model';

@Injectable({ providedIn: 'root' })
export class InventaireApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/inventaires`;
  private readonly passagesBase = `${environment.apiUrl}/passages`;

  getInventaires(page = 1, pageSize = 20): Observable<PagedInventaires> {
    return this.http.get<PagedInventaires>(this.base, { params: { page, pageSize } });
  }

  createInventaire(body: CreateInventaireRequest): Observable<CreateInventaireResult> {
    return this.http.post<CreateInventaireResult>(this.base, body);
  }

  createPassage(inventaireId: number, type: 'C1' | 'C2'): Observable<PassageDto> {
    return this.http.post<PassageDto>(this.passagesBase, { inventaireId, type });
  }

  getSuiviTempsReel(inventaireId: number): Observable<SuiviTempsReel> {
    return this.http.get<SuiviTempsReel>(`${this.base}/${inventaireId}/suivi-temps-reel`);
  }

  getAgents(): Observable<Agent[]> {
    return this.http.get<Agent[]>(`${environment.apiUrl}/agents`);
  }

  importStock(passageId: number, file: File, codeAgents: string[] = []): Observable<ImportStockResult> {
    const form = new FormData();
    form.append('file', file, file.name);
    if (codeAgents.length > 0) {
      form.append('codeAgents', codeAgents.join(','));
    }
    return this.http.post<ImportStockResult>(`${this.passagesBase}/${passageId}/import-stock`, form);
  }

  cloturerInventaire(inventaireId: number): Observable<CloturerInventaireResult> {
    return this.http.post<CloturerInventaireResult>(`${this.base}/${inventaireId}/cloturer`, {});
  }

  exportInventaire(inventaireId: number): Observable<Blob> {
    return this.http.get(`${this.base}/${inventaireId}/export`, { responseType: 'blob' });
  }
}
