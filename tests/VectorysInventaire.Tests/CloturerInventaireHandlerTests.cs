using FluentAssertions;
using Moq;
using VectorysInventaire.Application.Features.Inventaires.Commands.CloturerInventaire;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Tests;

public class CloturerInventaireHandlerTests
{
    [Fact]
    public async Task Cloture_Sets_Statut_And_DateFin()
    {
        var inventaire = new Inventaire
        {
            Id = 7,
            Reference = "INV-2026-001",
            Intitule = "Test",
            Zone = "A",
            DateDebut = DateTime.UtcNow.AddDays(-1),
            Statut = StatutInventaireEnum.EN_COURS,
            SuperviseurId = 1
        };

        var repo = new Mock<IInventaireRepository>();
        repo.Setup(r => r.GetByIdForUpdateAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventaire);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CloturerInventaireCommandHandler(repo.Object);
        var result = await handler.Handle(new CloturerInventaireCommand(7), CancellationToken.None);

        inventaire.Statut.Should().Be(StatutInventaireEnum.CLOTURE);
        inventaire.DateFin.Should().NotBeNull();
        result.Inventaire.Statut.Should().Be("CLOTURE");
        result.Message.Should().Contain("INV-2026-001");
    }

    [Fact]
    public async Task Cloture_Fails_When_Already_Closed()
    {
        var inventaire = new Inventaire
        {
            Id = 8,
            Reference = "INV-2026-002",
            Statut = StatutInventaireEnum.CLOTURE,
            DateDebut = DateTime.UtcNow
        };

        var repo = new Mock<IInventaireRepository>();
        repo.Setup(r => r.GetByIdForUpdateAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventaire);

        var handler = new CloturerInventaireCommandHandler(repo.Object);
        var act = () => handler.Handle(new CloturerInventaireCommand(8), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*déjà clôturé*");
    }

    [Fact]
    public async Task Cloture_Fails_When_EnPreparation()
    {
        var inventaire = new Inventaire
        {
            Id = 9,
            Reference = "INV-2026-003",
            Statut = StatutInventaireEnum.EN_PREPARATION,
            DateDebut = DateTime.UtcNow
        };

        var repo = new Mock<IInventaireRepository>();
        repo.Setup(r => r.GetByIdForUpdateAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventaire);

        var handler = new CloturerInventaireCommandHandler(repo.Object);
        var act = () => handler.Handle(new CloturerInventaireCommand(9), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*préparation*");
    }
}
