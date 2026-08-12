export interface Agent {
  id: number;
  nom: string;
  prenom: string;
  codePersonnel: string;
}

export function agentDisplayName(agent: Agent): string {
  const full = `${agent.prenom} ${agent.nom}`.trim();
  return full || agent.codePersonnel;
}
