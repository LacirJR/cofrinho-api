using cofrinho.application.Commands.Transacoes.Criar;
using cofrinho.core.Abstractions;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Entities;
using cofrinho.core.Enums;
using Moq;

namespace cofrinho.tests.Application.Commands;

public class CriarTransacoesCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;
    private readonly CriarTransacoesCommandHandler _handler;

    public CriarTransacoesCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
        _unitOfWorkMock.Setup(u => u.Transacoes).Returns(_transacaoRepositoryMock.Object);
        _handler = new CriarTransacoesCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComComandoValido_DeveAdicionarAoRepositorioEComitar()
    {
        // Arrange
        var command = new CriarTransacoesCommand(100, TipoMoedaEnum.BRL, TipoTransacaoEnum.Deposito, DateTime.UtcNow) 
        { 
            ObjetivoId = Guid.NewGuid() 
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _transacaoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ComComandoInvalido_NaoDeveAdicionarAoRepositorioNemComitar()
    {
        // Arrange
        var command = new CriarTransacoesCommand(-100, TipoMoedaEnum.BRL, TipoTransacaoEnum.Deposito, DateTime.UtcNow) 
        { 
            ObjetivoId = Guid.NewGuid() 
        }; // Valor inválido

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _transacaoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }
}
