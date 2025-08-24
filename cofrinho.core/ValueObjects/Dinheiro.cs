using cofrinho.core.Common;
using cofrinho.core.Enums;
using Flunt.Notifications;
using Flunt.Validations;

namespace cofrinho.core.ValueObjects;

public class Dinheiro : Notifiable<Notification>
{
    public decimal Valor { get; }
    public TipoMoedaEnum MoedaEnum { get; }

    private Dinheiro(decimal valor, TipoMoedaEnum moedaEnum)
    {
        Valor = valor;
        MoedaEnum = moedaEnum;

        Validar();
    }

    private void Validar()
    {
        var contrato = new Contract<Dinheiro>()
            .Requires()
            .IsGreaterThan(Valor, 0, "Valor", "O valor não pode ser negativo.")
            .IsNotNull(MoedaEnum, "Moeda", "A moeda é obrigatória.");

        AddNotifications(contrato);
    }

    public static Dinheiro Criar(decimal valor, TipoMoedaEnum moedaEnum)
    {
        var dinheiro = new Dinheiro(valor, moedaEnum);
        return dinheiro;
    }

    public Dinheiro Somar(Dinheiro outro)
    {
        var contrato = new Contract<Dinheiro>()
            .Requires()
            .AreEquals(MoedaEnum, outro.MoedaEnum, "Moeda", "Não é possível somar moedas diferentes.");

        AddNotifications(contrato);

        if (IsValid)
            return new(Valor + outro.Valor, MoedaEnum);

        return this;
    }

    public Dinheiro Subtrair(Dinheiro outro)
    {
        var contrato = new Contract<Dinheiro>()
            .Requires()
            .AreEquals(MoedaEnum, outro.MoedaEnum, "Moeda", "Não é possível somar moedas diferentes.");

        AddNotifications(contrato);

        if (IsValid)
            return new(Valor - outro.Valor, MoedaEnum);

        return this;
    }
}