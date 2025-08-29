using cofrinho.application.Queries.Objetivos.BuscarPorId;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Entities;
using cofrinho.core.Enums;
using cofrinho.core.Specifications;
using MapsterMapper;
using Moq;

namespace cofrinho.tests.Application.Queries;

public class BuscarObjetivoPorIdQueryHandlerTests
{
    private readonly Mock<IObjetivoRepository> _objetivoRepositoryMock;
    private readonly BuscarObjetivoPorIdCommandHandler _handler;

    public BuscarObjetivoPorIdQueryHandlerTests()
    {
        _objetivoRepositoryMock = new Mock<IObjetivoRepository>();
        // O handler real tem uma dependência de IMapper que não é usada no método Handle.
        // Passamos null aqui, o que também revela uma pequena oportunidade de refatoração no futuro.
        _handler = new BuscarObjetivoPorIdCommandHandler(_objetivoRepositoryMock.Object, null);
    }

    [Fact]
    public async Task Handle_QuandoObjetivoExiste_DeveRetornarViewModelComSucesso()
    {
        // Arrange
        var objetivoId = Guid.NewGuid();
        var objetivo = Objetivo.Criar("Teste", "Desc", 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);
        
        _objetivoRepositoryMock
            .Setup(r => r.SingleOrDefaultAsync(It.IsAny<BuscarObjetivoComTransacoesSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(objetivo);

        var command = new BuscarObjetivoPorIdCommand(objetivoId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(objetivo.Id, result.Data.Id);
        Assert.Equal(objetivo.Titulo, result.Data.Titulo);
    }

    [Fact]
    public async Task Handle_QuandoObjetivoNaoExiste_DeveRetornarErro()
    {
        // Arrange
        _objetivoRepositoryMock
            .Setup(r => r.SingleOrDefaultAsync(It.IsAny<BuscarObjetivoComTransacoesSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Objetivo)null);

        var command = new BuscarObjetivoPorIdCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Contains("Não foi possível encontrar o objetivo.", result.Messages);
    }
}
