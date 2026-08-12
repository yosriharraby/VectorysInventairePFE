export interface Inventaire {
  id: number;
  reference: string;
  intitule: string;
  zone: string;
  dateDebut: string;
  dateFin?: string | null;
  statut: string;
  superviseurId: number;
  createdAt: string;
}

export interface PagedInventaires {
  items: Inventaire[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateInventaireRequest {
  intitule: string;
  zone: string;
  dateDebut: string;
  dateFin?: string | null;
}

export interface CreateInventaireResult {
  inventaire: Inventaire;
  passageC1Id: number;
}

export interface ImportStockResult {
  emplacementsCrees: number;
  lignesStockImportees: number;
  assignationsParAgent?: Record<string, number>;
  agentsInconnus?: string[];
}
export interface CloturerInventaireResult {
  inventaire: Inventaire;
  message: string;
}
export interface PassageDto {
  id: number;
  inventaireId: number;
  type: string;
  numero: number;
  statut: string;
}