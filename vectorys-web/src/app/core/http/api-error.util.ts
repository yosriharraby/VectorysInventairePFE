export function apiErrorMessage(err: unknown, fallback: string): string {
  const status = (err as { status?: number })?.status;
  if (status === 0 || status === undefined) {
    return 'API non disponible. Démarrez l\'API sur http://localhost:5000 puis rechargez la page.';
  }
  const message = (err as { error?: { message?: string } })?.error?.message;
  return message ?? fallback;
}
