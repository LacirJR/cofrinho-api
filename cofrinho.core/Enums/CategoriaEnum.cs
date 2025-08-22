using System.ComponentModel;

namespace cofrinho.core.Enums;

public enum CategoriaEnum
{
    [Description("Viagem")] Viagem,
    [Description("Emergência")] Emergencia,
    [Description("Educação")] Educacao,
    [Description("Casa")] Casa,
    [Description("Carro")] Carro,
    [Description("Saúde")] Saude,
    [Description("Casamento")] Casamento,
    [Description("Aposentadoria")] Aposentadoria,
    [Description("Tecnologia / Eletrônicos")] Tecnologia,
    [Description("Lazer e Hobbies")] Lazer,
    [Description("Presentes e Festas")] Presentes,
    [Description("Investimentos")] Investimentos
}