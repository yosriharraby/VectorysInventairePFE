using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Application.Features.Ecarts;

internal static class EcartBusinessKey
{
    public static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    public static string Of(string codeEmplacement, string codeUS) =>
        $"{codeEmplacement.Trim()}|{codeUS.Trim()}";

    public static string Of(Ecart ecart) => Of(ecart.CodeEmplacement, ecart.CodeUS);

    public static HashSet<string> Traites(IEnumerable<Ecart> ecarts) =>
        ecarts
            .Where(e => e.Statut is StatutEcartEnum.TRAITE_BRANCHE_A or StatutEcartEnum.TRAITE_BRANCHE_B)
            .Select(Of)
            .ToHashSet(Comparer);

    public static HashSet<string> TraitesBrancheB(IEnumerable<Ecart> ecarts) =>
        ecarts
            .Where(e => e.Statut == StatutEcartEnum.TRAITE_BRANCHE_B)
            .Select(Of)
            .ToHashSet(Comparer);
}
