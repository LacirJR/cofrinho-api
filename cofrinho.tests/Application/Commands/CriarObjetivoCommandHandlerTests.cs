using cofrinho.application.Commands.Objetivos.Criar;
using cofrinho.core.Abstractions;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Entities;
using cofrinho.core.Enums;
using Moq;

namespace cofrinho.tests.Application.Commands;

public class CriarObjetivoCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IObjetivoRepository> _objetivoRepositoryMock;
    private readonly CriarObjetivoCommandHandler _handler;

    public CriarObjetivoCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _objetivoRepositoryMock = new Mock<IObjetivoRepository>();
        _unitOfWorkMock.Setup(u => u.Objetivos).Returns(_objetivoRepositoryMock.Object);
        _handler = new CriarObjetivoCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComComandoValido_DeveAdicionarAoRepositorioEComitar()
    {
        // Arrange
        var command = new CriarObjetivoCommand
        {
            Titulo = "Carro Novo",
            Descricao = "Guardar dinheiro para um carro",
            ValorAlvo = 50000,
            TipoMoeda = TipoMoedaEnum.BRL,
            Categoria = CategoriaEnum.Carro
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _objetivoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Objetivo>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ComComandoInvalido_NaoDeveAdicionarAoRepositorioNemComitar()
    {
        // Arrange
        var command = new CriarObjetivoCommand
        {
            Titulo = "", // Título inválido
            Descricao = "Invalido",
            ValorAlvo = 100,
            TipoMoeda = TipoMoedaEnum.BRL,
            Categoria = CategoriaEnum.Lazer
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _objetivoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Objetivo>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }
}
