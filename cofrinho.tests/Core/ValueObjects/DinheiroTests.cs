using cofrinho.core.Enums;
using cofrinho.core.ValueObjects;

namespace cofrinho.tests.Core.ValueObjects;

public class DinheiroTests
{
    [Fact]
    public void Criar_ComValorNegativo_DeveRetornarInvalido()
    {
        // Arrange
        var valor = -100m;

        // Act
        var dinheiro = Dinheiro.Criar(valor, TipoMoedaEnum.BRL);

        // Assert
        Assert.False(dinheiro.IsValid);
        Assert.Contains(dinheiro.Notifications, n => n.Message == "O valor não pode ser negativo.");
    }

    [Fact]
    public void Somar_ComMoedasIguais_DeveRetornarSomaCorreta()
    {
        // Arrange
        var d1 = Dinheiro.Criar(100, TipoMoedaEnum.BRL);
        var d2 = Dinheiro.Criar(50, TipoMoedaEnum.BRL);

        // Act
        var resultado = d1.Somar(d2);

        // Assert
        Assert.True(resultado.IsValid);
        Assert.Equal(150, resultado.Valor);
    }
    
    [Fact]
    public void Subtrair_ComMoedasIguais_DeveRetornarSubtracaoCorreta()
    {
        // Arrange
        var d1 = Dinheiro.Criar(100, TipoMoedaEnum.BRL);
        var d2 = Dinheiro.Criar(50, TipoMoedaEnum.BRL);

        // Act
        var resultado = d1.Subtrair(d2);

        // Assert
        Assert.True(resultado.IsValid);
        Assert.Equal(50, resultado.Valor);
    }
    
    [Fact]
    public void Somar_ComMoedasDiferentes_DeveRetornarInvalido()
    {
        // Arrange
        var d1 = Dinheiro.Criar(100, TipoMoedaEnum.BRL);
        var d2 = Dinheiro.Criar(50, TipoMoedaEnum.USD);

        // Act
        var resultado = d1.Somar(d2);

        // Assert
        Assert.False(resultado.IsValid);
        Assert.Contains(d1.Notifications, n => n.Message == "Não é possível somar moedas diferentes.");
    }
}
