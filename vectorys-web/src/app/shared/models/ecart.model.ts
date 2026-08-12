export interface Ecart {
  id: number;
  inventaireId: number;
  emplacementId: number;
  codeEmplacement: string;
  designationEmplacement: string;
  codeUS: string;
  designationUS: string;
  qteTheorique: number;
  qteScannee: number;
  delta: number;
  type: 'MANQUANT' | 'SURPLUS';
  statut: 'DETECTE' | 'TRAITE_BRANCHE_A' | 'TRAITE_BRANCHE_B';
  detecteAt: string;
  traiteAt?: string | null;
}

export interface EcartsList {
  inventaireId: number;
  inventaireReference: string;
  totalDetectes: number;
  totalTraites: number;
  totalManquants: number;
  totalSurplus: number;
  items: Ecart[];
}

export interface DetecterEcartsResult {
  inventaireId: number;
  ecartsDetectes: number;
  items: Ecart[];
}

export interface AppliquerBrancheARequest {
  inventaireId: number;
  ecartIds: number[];
}

export interface AppliquerBrancheAResult {
  inventaireId: number;
  passageC2Id: number;
  ecartsTraites: number;
  emplacementsCreesC2: number;
  message: string;
}

export interface AppliquerBrancheBRequest {
  inventaireId: number;
}

export interface AppliquerBrancheBResult {
  inventaireId: number;
  passageC2Id: number;
  ecartsTraites: number;
  emplacementsCreesC2: number;
  message: string;
}

export interface EcartsCount {
  totalDetectes: number;
}
