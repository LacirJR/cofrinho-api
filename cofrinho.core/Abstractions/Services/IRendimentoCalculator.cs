
namespace cofrinho.core.Abstractions.Services;

public interface IRendimentoCalculator
{
    decimal CalcularTaxaDiaria(decimal taxaAnual);
    decimal CalcularRendimentoDiario(decimal saldoBase, decimal taxaAnual);
}
