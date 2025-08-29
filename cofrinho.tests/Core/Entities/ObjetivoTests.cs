using cofrinho.core.Entities;
using cofrinho.core.Enums;
using cofrinho.core.ValueObjects;

namespace cofrinho.tests.Core.Entities;

public class ObjetivoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarObjetivoComStatusEmProgresso()
    {
        // Arrange
        var titulo = "Viagem para o Japão";
        var descricao = "Economizar para viagem em 2026";
        var valorAlvo = 20000m;
        var tipoMoeda = TipoMoedaEnum.BRL;
        var prazo = new DateTime(2026, 10, 1);
        var categoria = CategoriaEnum.Viagem;

        // Act
        var objetivo = Objetivo.Criar(titulo, descricao, valorAlvo, tipoMoeda, prazo, categoria);

        // Assert
        Assert.True(objetivo.IsValid);
        Assert.Equal(titulo, objetivo.Titulo);
        Assert.Equal(StatusObjetivoEnum.EmProgresso, objetivo.Status);
    }

    [Fact]
    public void Criar_ComTituloInvalido_DeveRetornarInvalido()
    {
        // Arrange & Act
        var objetivo = Objetivo.Criar("", "", 0, TipoMoedaEnum.BRL, DateTime.Now, CategoriaEnum.Lazer);

        // Assert
        Assert.False(objetivo.IsValid);
        Assert.Contains(objetivo.Notifications, n => n.Key == "Titulo");
    }

    [Fact]
    public void AlterarTitulo_ComValorInvalido_DeveAdicionarNotificacao()
    {
        // Arrange
        var objetivo = Objetivo.Criar("Titulo Valido", null, 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);
        
        // Act
        objetivo.AlterarTitulo("a");
        
        // Assert
        Assert.False(objetivo.IsValid);
        Assert.Contains(objetivo.Notifications, n => n.Key == "Titulo");
    }

    [Fact]
    public void AdicionarTransacao_ComTransacaoValida_DeveAdicionarNaLista()
    {
        // Arrange
        var objetivo = Objetivo.Criar("Teste", null, 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);
        var transacao = Transacao.Criar(Dinheiro.Criar(50, TipoMoedaEnum.BRL), TipoTransacaoEnum.Deposito, DateTime.UtcNow, objetivo.Id);
        
        // Act
        objetivo.AdicionarTransacao(transacao);
        
        // Assert
        Assert.Single(objetivo.Transacoes);
        Assert.Equal(transacao, objetivo.Transacoes.First());
    }

    [Fact]
    public void AlterarPrazo_ComDataNoPassado_DeveRetornarInvalido()
    {
        // Arrange
        var objetivo = Objetivo.Criar("Teste", null, 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);
        var prazoAntigo = DateTime.UtcNow.AddDays(-1);

        // Act
        objetivo.AlterarPrazo(prazoAntigo);

        // Assert
        Assert.False(objetivo.IsValid);
        Assert.Contains(objetivo.Notifications, n => n.Key == "Prazo");
    }

    [Fact]
    public void AlterarStatus_DeUmObjetivoCancelado_DeveRetornarInvalido()
    {
        // Arrange
        var objetivo = Objetivo.Criar("Teste", null, 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);
        objetivo.AlterarStatus(StatusObjetivoEnum.Cancelado); // Cancela primeiro

        // Act
        objetivo.AlterarStatus(StatusObjetivoEnum.EmProgresso); // Tenta reativar

        // Assert
        Assert.False(objetivo.IsValid);
        Assert.Contains(objetivo.Notifications, n => n.Message.Contains("Não pode alterar status de um objetivo cancelado"));
    }
    
    [Fact]
    public void Deletar_QuandoChamado_DeveMarcarComoDeletado()
    {
        // Arrange
        var objetivo = Objetivo.Criar("Teste", null, 100, TipoMoedaEnum.BRL, null, CategoriaEnum.Lazer);

        // Act
        objetivo.Deletar();

        // Assert
        Assert.True(objetivo.EstaDeletado);
    }
}