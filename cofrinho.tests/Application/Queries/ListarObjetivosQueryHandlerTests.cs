using cofrinho.application.Models;
using cofrinho.application.Models.Objetivos;
using cofrinho.application.Queries.Objetivos.Listar;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Entities;
using cofrinho.core.Enums;
using cofrinho.core.Specifications;
using MapsterMapper;
using Moq;

namespace cofrinho.tests.Application.Queries;

public class ListarObjetivosQueryHandlerTests
{
    private readonly Mock<IObjetivoRepository> _objetivoRepositoryMock;
    private readonly ListarObjetivosQueryHandler _handler;

    public ListarObjetivosQueryHandlerTests()
    {
        _objetivoRepositoryMock = new Mock<IObjetivoRepository>();
        // A dependência do IMapper não é utilizada no handler, então passamos null.
        _handler = new ListarObjetivosQueryHandler(_objetivoRepositoryMock.Object, null);
    }

    [Fact]
    public async Task Handle_QuandoExistemObjetivos_DeveRetornarPagedResult()
    {
        // Arrange
        var objetivos = new List<Objetivo>
        {
            Objetivo.Criar("Obj 1", null, 1, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer),
            Objetivo.Criar("Obj 2", null, 2, TipoMoedaEnum.BRL, null, CategoriaEnum.Viagem)
        };

        _objetivoRepositoryMock.Setup(r => r.CountAsync(It.IsAny<ListagemObjetivosSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(objetivos.Count);
        
        _objetivoRepositoryMock.Setup(r => r.ListAsync(It.IsAny<ListagemObjetivosSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(objetivos);

        var query = new ListarObjetivosQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalItems);
        Assert.Equal(2, result.Data.Items.Count());
        Assert.Equal("Obj 1", result.Data.Items.First().Titulo);
    }
    
    [Fact]
    public async Task Handle_QuandoNaoExistemObjetivos_DeveRetornarPagedResultVazio()
    {
        // Arrange
        var objetivos = new List<Objetivo>();

        _objetivoRepositoryMock.Setup(r => r.CountAsync(It.IsAny<ListagemObjetivosSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        
        _objetivoRepositoryMock.Setup(r => r.ListAsync(It.IsAny<ListagemObjetivosSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(objetivos);

        var query = new ListarObjetivosQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(0, result.Data.TotalItems);
        Assert.Empty(result.Data.Items);
    }
}
