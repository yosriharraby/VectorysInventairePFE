using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;

/// <summary>
/// Mode GLOBAL : recomptage dès le début via Excel filtré (périmètre C2 = fichier).
/// Mode CIBLE : Excel restreint aux emplacements déjà présents dans l'import C1 initial.
/// </summary>
public record AppliquerBrancheBCommand(
    int InventaireId,
    string Mode,
    Stream FileStream,
    string? FichierNom = null,
    string? AssignationMode = null,
    string? AgentCode = null,
    int? SuperviseurId = null) : IRequest<AppliquerBrancheBResultDto>;

public static class BrancheBModes
{
    public const string Global = "GLOBAL";
    public const string Cible = "CIBLE";

    public static bool IsValid(string? mode) =>
        string.Equals(mode, Global, StringComparison.OrdinalIgnoreCase)
        || string.Equals(mode, Cible, StringComparison.OrdinalIgnoreCase);

    public static string Normalize(string mode) =>
        string.Equals(mode, Cible, StringComparison.OrdinalIgnoreCase) ? Cible : Global;
}

public static class AssignationRecomptageModes
{
    public const string Excel = "EXCEL";
    public const string MemeAgent = "MEME_AGENT";
    public const string NouveauAgent = "NOUVEAU_AGENT";
    public const string TousAgents = "TOUS_AGENTS";

    public static bool IsValid(string? mode) =>
        string.IsNullOrWhiteSpace(mode)
        || string.Equals(mode, Excel, StringComparison.OrdinalIgnoreCase)
        || string.Equals(mode, MemeAgent, StringComparison.OrdinalIgnoreCase)
        || string.Equals(mode, NouveauAgent, StringComparison.OrdinalIgnoreCase)
        || string.Equals(mode, TousAgents, StringComparison.OrdinalIgnoreCase);

    public static string Normalize(string? mode, string brancheBMode)
    {
        _ = brancheBMode;
        if (string.IsNullOrWhiteSpace(mode))
            return Excel;

        if (string.Equals(mode, MemeAgent, StringComparison.OrdinalIgnoreCase)) return MemeAgent;
        if (string.Equals(mode, NouveauAgent, StringComparison.OrdinalIgnoreCase)) return NouveauAgent;
        if (string.Equals(mode, TousAgents, StringComparison.OrdinalIgnoreCase)) return TousAgents;
        return Excel;
    }
}
