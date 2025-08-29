using cofrinho.infrastructure.Background.Rendimento;

namespace cofrinho.tests;

public class RendimentoCalculatorTests
{
    private readonly RendimentoCalculator _calculator = new();

    [Fact]
    public void CalcularTaxaDiaria_ComTaxaAnualValida_DeveRetornarTaxaDiariaCorreta()
    {
        // Arrange
        var taxaAnual = 0.12m; // 12%
        var taxaDiariaEsperada = 0.00044982m; // Valor correto para (1 + 0.12)^(1/252) - 1

        // Act
        var resultado = _calculator.CalcularTaxaDiaria(taxaAnual);

        // Assert
        Assert.Equal(taxaDiariaEsperada, resultado, 8); // 8 casas de precisão
    }

    [Theory]
    [InlineData(1000, 0.10, 0.38)] // Saldo, Taxa Anual, Rendimento Diário Esperado
    [InlineData(550.50, 0.1325, 0.27)] // Valor correto para 550.50 * ((1+0.1325)^(1/252)-1)
    [InlineData(100, 0.0, 0.0)]
    public void CalcularRendimentoDiario_ComValoresDiferentes_DeveRetornarRendimentoCorreto(double saldo, double taxaAnual, double esperado)
    {
        // Arrange
        var saldoDecimal = (decimal)saldo;
        var taxaAnualDecimal = (decimal)taxaAnual;
        var esperadoDecimal = (decimal)esperado;

        // Act
        var resultado = _calculator.CalcularRendimentoDiario(saldoDecimal, taxaAnualDecimal);

        // Assert
        Assert.Equal(esperadoDecimal, resultado, 2);
    }
}