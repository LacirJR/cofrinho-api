using cofrinho.application.Commands.Objetivos.Remover;
using cofrinho.core.Abstractions;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Entities;
using cofrinho.core.Enums;
using Moq;

namespace cofrinho.tests.Application.Commands;

public class RemoverObjetivoCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IObjetivoRepository> _objetivoRepositoryMock;
    private readonly RemoverObjetivoCommandHandler _handler;

    public RemoverObjetivoCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _objetivoRepositoryMock = new Mock<IObjetivoRepository>();
        _unitOfWorkMock.Setup(u => u.Objetivos).Returns(_objetivoRepositoryMock.Object);
        _handler = new RemoverObjetivoCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComIdValido_DeveChamarDeletarEComitar()
    {
        // Arrange
        var objetivo = Objetivo.Criar("Teste", null, 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);
        var objetivoId = objetivo.Id;
        _objetivoRepositoryMock.Setup(r => r.GetByIdAsync(objetivoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(objetivo);

        var command = new RemoverObjetivoCommand(objetivoId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(objetivo.EstaDeletado); // Verifica se o método Deletar foi eficaz
        _objetivoRepositoryMock.Verify(r => r.UpdateAsync(objetivo, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoObjetivoNaoEncontrado_DeveRetornarErro()
    {
        // Arrange
        var objetivoId = Guid.NewGuid();
        _objetivoRepositoryMock.Setup(r => r.GetByIdAsync(objetivoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Objetivo)null);
        
        var command = new RemoverObjetivoCommand(objetivoId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Objetivo não encontrado", result.Messages);
        _objetivoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Objetivo>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }
}
