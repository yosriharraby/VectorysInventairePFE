using FluentAssertions;
using FluentValidation.TestHelper;
using VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;
using VectorysInventaire.Application.Features.Inventaires.Commands.CloturerInventaire;
using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Tests;

public class Sprint6ValidatorsTests
{
    [Fact]
    public void AppliquerBrancheB_Rejects_NonPositive_InventaireId()
    {
        var validator = new AppliquerBrancheBCommandValidator();
        using var stream = new MemoryStream(new byte[] { 1 });
        var result = validator.TestValidate(new AppliquerBrancheBCommand(0, BrancheBModes.Global, stream));
        result.ShouldHaveValidationErrorFor(x => x.InventaireId);
    }

    [Fact]
    public void AppliquerBrancheB_Rejects_Invalid_Mode()
    {
        var validator = new AppliquerBrancheBCommandValidator();
        using var stream = new MemoryStream(new byte[] { 1 });
        var result = validator.TestValidate(new AppliquerBrancheBCommand(12, "XYZ", stream));
        result.ShouldHaveValidationErrorFor(x => x.Mode);
    }

    [Fact]
    public void AppliquerBrancheB_Accepts_Valid_Global()
    {
        var validator = new AppliquerBrancheBCommandValidator();
        using var stream = new MemoryStream(new byte[] { 1 });
        var result = validator.TestValidate(new AppliquerBrancheBCommand(12, BrancheBModes.Global, stream));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void AppliquerBrancheB_Accepts_Valid_Cible()
    {
        var validator = new AppliquerBrancheBCommandValidator();
        using var stream = new MemoryStream(new byte[] { 1 });
        var result = validator.TestValidate(new AppliquerBrancheBCommand(12, BrancheBModes.Cible, stream));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CloturerInventaire_Rejects_NonPositive_InventaireId()
    {
        var validator = new CloturerInventaireCommandValidator();
        var result = validator.TestValidate(new CloturerInventaireCommand(-1));
        result.ShouldHaveValidationErrorFor(x => x.InventaireId);
    }

    [Fact]
    public void CloturerInventaire_Accepts_Valid_InventaireId()
    {
        var validator = new CloturerInventaireCommandValidator();
        var result = validator.TestValidate(new CloturerInventaireCommand(3));
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class DomainEnumsTests
{
    [Fact]
    public void StatutEcartEnum_Includes_BrancheB()
    {
        Enum.IsDefined(typeof(StatutEcartEnum), StatutEcartEnum.TRAITE_BRANCHE_B).Should().BeTrue();
        ((int)StatutEcartEnum.TRAITE_BRANCHE_B).Should().Be(3);
    }

    [Fact]
    public void StatutInventaireEnum_Includes_Cloture()
    {
        Enum.IsDefined(typeof(StatutInventaireEnum), StatutInventaireEnum.CLOTURE).Should().BeTrue();
        ((int)StatutInventaireEnum.CLOTURE).Should().Be(3);
    }
}
