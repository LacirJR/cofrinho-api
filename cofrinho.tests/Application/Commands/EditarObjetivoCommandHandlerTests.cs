using cofrinho.application.Commands.Objetivos.Editar;
using cofrinho.core.Abstractions;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Entities;
using cofrinho.core.Enums;
using Moq;

namespace cofrinho.tests.Application.Commands;

public class EditarObjetivoCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IObjetivoRepository> _objetivoRepositoryMock;
    private readonly EditarObjetivoByIdCommandHandler _handler;

    public EditarObjetivoCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _objetivoRepositoryMock = new Mock<IObjetivoRepository>();
        _unitOfWorkMock.Setup(u => u.Objetivos).Returns(_objetivoRepositoryMock.Object);
        _handler = new EditarObjetivoByIdCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComComandoValido_DeveAtualizarEComitar()
    {
        // Arrange
        var objetivo = Objetivo.Criar("Titulo Antigo", "Desc", 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);
        var objetivoId = objetivo.Id;
        _objetivoRepositoryMock.Setup(r => r.GetByIdAsync(objetivoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(objetivo);

        var command = new EditarObjetivoByIdCommand(objetivoId, "Titulo Novo", null, null, null, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _objetivoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Objetivo>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoObjetivoNaoEncontrado_DeveRetornarErro()
    {
        // Arrange
        var objetivoId = Guid.NewGuid();
        _objetivoRepositoryMock.Setup(r => r.GetByIdAsync(objetivoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Objetivo)null);
        
        var command = new EditarObjetivoByIdCommand(objetivoId, "Titulo Novo", null, null, null, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Não foi possível encontrar o objetivo.", result.Messages);
        _objetivoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Objetivo>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ComDadosInvalidos_NaoDeveAtualizarNemComitar()
    {
        // Arrange
        var objetivo = Objetivo.Criar("Titulo Antigo", "Desc", 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);
        var objetivoId = objetivo.Id;
        _objetivoRepositoryMock.Setup(r => r.GetByIdAsync(objetivoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(objetivo);

        var command = new EditarObjetivoByIdCommand(objetivoId, "a", null, null, null, null, null); // Titulo inválido

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _objetivoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Objetivo>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }
}
