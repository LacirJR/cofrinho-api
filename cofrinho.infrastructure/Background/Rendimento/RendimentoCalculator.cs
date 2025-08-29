using System;
using cofrinho.core.Abstractions.Services;

namespace cofrinho.infrastructure.Background.Rendimento;

public class RendimentoCalculator : IRendimentoCalculator
{
    private const int DiasUteisAno = 252;

    public decimal CalcularTaxaDiaria(decimal taxaAnual)
    {
        // taxa_diaria = (1 + taxa_anual)^(1/252) - 1
        var anual = (double)taxaAnual;
        var diaria = Math.Pow(1.0 + anual, 1.0 / DiasUteisAno) - 1.0;
        return (decimal)diaria;
    }

    public decimal CalcularRendimentoDiario(decimal saldoBase, decimal taxaAnual)
    {
        var taxaDiaria = CalcularTaxaDiaria(taxaAnual);
        var rendimento = saldoBase * taxaDiaria;
        // Arredondamento bancário (to even) para 2 casas decimais
        return Math.Round(rendimento, 2, MidpointRounding.ToEven);
    }
}