export interface Ecart {
  id: number;
  inventaireId: number;
  emplacementId: number;
  codeEmplacement: string;
  designationEmplacement: string;
  codeUS: string;
  designationUS: string;
  client?: string | null;
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
  tousRecomptagesCorriges?: boolean;
  items: Ecart[];
}

export interface DetecterEcartsResult {
  inventaireId: number;
  ecartsDetectes: number;
  ecartsDejaTraitesNonRouverts?: number;
  message?: string;
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

/** GLOBAL = refaire dès le début ; CIBLE = emplacements ⊆ import C1 initial */
export type BrancheBMode = 'GLOBAL' | 'CIBLE';

export interface AppliquerBrancheBResult {
  inventaireId: number;
  passageC2Id: number;
  recomptageId: number;
  ecartsTraites: number;
  emplacementsCreesC2: number;
  message: string;
}

export type AssignationRecomptageMode = 'EXCEL' | 'MEME_AGENT' | 'NOUVEAU_AGENT' | 'TOUS_AGENTS';

export interface RecomptageAgent {
  agentId: number;
  codePersonnel: string;
  nomComplet: string;
  emplacementsAssignes: number;
}

export interface Recomptage {
  id: number;
  inventaireId: number;
  passageC2Id: number;
  mode: BrancheBMode;
  assignationMode?: string | null;
  fichierNom?: string | null;
  superviseurNom: string;
  emplacementsCrees: number;
  ecartsTraites: number;
  ecartsRestants?: number;
  statut: string;
  createdAt: string;
  emplacementCodes?: string[];
  agents: RecomptageAgent[];
}

export interface RecomptagesList {
  inventaireId: number;
  inventaireReference: string;
  totalRecomptages: number;
  items: Recomptage[];
}

export interface EcartsCount {
  totalDetectes: number;
}
