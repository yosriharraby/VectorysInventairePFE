export interface EmplacementSuivi {
  id: number;
  code: string;
  designation: string;
  statut: string;
  qteScannee: number;
  qteAttendue: number;
  dernierScan?: string | null;
}

export interface AgentSuivi {
  agentId: number;
  nom: string;
  codePersonnel?: string | null;
  estActif: boolean;
  emplacementsTermines: number;
  emplacementsTotal: number;
  emplacements: EmplacementSuivi[];
}

export interface SuiviTempsReel {
  inventaireId: number;
  intitule: string;
  passageC1Id: number;
  agents: AgentSuivi[];
}

export interface NouveauScanEvent {
  inventaireId: number;
  emplacementId: number;
  emplacementCode: string;
  emplacementDesignation: string;
  agentId: number;
  agentNom: string;
  codeBarresScan: string;
  qteScannee: number;
  qteAttendue: number;
  emplacementStatut: string;
  dateHeureScan: string;
}

export interface FinComptageEvent {
  inventaireId: number;
  emplacementId: number;
  emplacementCode: string;
  agentId: number;
  agentNom: string;
  statut: string;
  qteScannee: number;
  qteAttendue: number;
}
