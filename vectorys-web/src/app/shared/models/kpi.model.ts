export interface KpiFilter {
  dateFrom?: string;
  dateTo?: string;
  inventaireId?: number | null;
  zone?: string;
}

export interface KpiSeriesPoint {
  label: string;
  value: number;
}

export interface KpiOperatorProductivity {
  agentId: number;
  nom: string;
  unitesParHeure: number;
  emplacementsParHeure: number;
  totalScans: number;
  emplacementsTermines: number;
}

export interface KpiClientZone {
  zone: string;
  inventairesRealises: number;
  tauxFiabiliteStock: number;
  valeurEcartsUnites: number;
  tempsInventairesHeures: number;
  feasibility: string;
  note?: string;
}

export interface KpiFeasibilityNote {
  kpiCode: string;
  label: string;
  status: string;
  formula: string;
  missingData?: string;
  scaleRecommendation?: string;
}

export interface KpiPrecision {
  nombreEcarts: number;
  tauxErreurs: number;
  lignesStockControlees: number;
  ecartsParType: KpiSeriesPoint[];
  ecartsParJour: KpiSeriesPoint[];
}

export interface KpiPerformance {
  inventairesRealises: number;
  inventairesEnCours: number;
  emplacementsTotal: number;
  emplacementsTermines: number;
  scansTotal: number;
  tauxAvancement: number;
  tempsMoyenRealisationHeures: number;
  referencesCompteesParHeure: number;
  productiviteOperateurs: KpiOperatorProductivity[];
  avancementParInventaire: KpiSeriesPoint[];
}

export interface KpiQualite {
  correctionsStock: number;
  tauxValidationPremierComptage: number;
  recomptagesNecessaires: number;
  tauxAnomaliesDetectees: number;
  tauxConformiteEmplacements: number;
  indicateurs: KpiSeriesPoint[];
}

export interface KpiClients {
  proxyUsed: string;
  parZone: KpiClientZone[];
}

export interface KpiWms {
  unitesInventorieesParJour: KpiSeriesPoint[];
  emplacementsInventoriesParJour: KpiSeriesPoint[];
  tauxOccupationEntrepot: number;
  delaiMoyenDetectionCorrectionHeures: number;
  occupationNote: string;
}

export interface KpiDashboard {
  filtresAppliques: KpiFilter;
  generatedAt: string;
  hasData: boolean;
  emptyDataMessage?: string | null;
  precision: KpiPrecision;
  performance: KpiPerformance;
  qualite: KpiQualite;
  clients: KpiClients;
  wms: KpiWms;
  feasibilityNotes: KpiFeasibilityNote[];
}
