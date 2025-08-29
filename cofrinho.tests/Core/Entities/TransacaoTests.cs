using cofrinho.core.Entities;
using cofrinho.core.Enums;
using cofrinho.core.ValueObjects;

namespace cofrinho.tests.Core.Entities;

public class TransacaoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarTransacaoValida()
    {
        // Arrange
        var dinheiro = Dinheiro.Criar(100, TipoMoedaEnum.BRL);
        var tipo = TipoTransacaoEnum.Deposito;
        var objetivoId = Guid.NewGuid();

        // Act
        var transacao = Transacao.Criar(dinheiro, tipo, DateTime.UtcNow, objetivoId);

        // Assert
        Assert.True(transacao.IsValid);
        Assert.Equal(100, transacao.Valor.Valor);
    }

    [Fact]
    public void Criar_ComDinheiroInvalido_DevePropagarNotificacoes()
    {
        // Arrange
        var dinheiro = Dinheiro.Criar(-100, TipoMoedaEnum.BRL); // Dinheiro inválido
        var tipo = TipoTransacaoEnum.Deposito;
        var objetivoId = Guid.NewGuid();

        // Act
        var transacao = Transacao.Criar(dinheiro, tipo, DateTime.UtcNow, objetivoId);

        // Assert
        Assert.False(transacao.IsValid);
        Assert.Contains(transacao.Notifications, n => n.Key == "Valor");
    }
}
