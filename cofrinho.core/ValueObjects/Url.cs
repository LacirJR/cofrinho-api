using cofrinho.core.Common;
using Flunt.Notifications;
using Flunt.Validations;

namespace cofrinho.core.ValueObjects;

public class Url : Notifiable<Notification>
{
    public string Valor { get; }

    private Url(string valor)
    {
        Valor = valor;
        Validar();
    }

    public static Url Criar(string valor)
    {
        var url = new Url(valor);

        return url;
    }

    private void Validar()
    {
        var contrato = new Contract<Url>()
            .IsNullOrWhiteSpace(Valor, nameof(Valor), "Url não pode ser nula ou vazia.")
            .IsUrl(Valor, nameof(Valor), "Formato URL inválido.");

        AddNotifications(contrato);
    }
}