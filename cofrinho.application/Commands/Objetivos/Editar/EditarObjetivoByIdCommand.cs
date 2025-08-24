using System.Text.Json.Serialization;
using cofrinho.application.Models;
using cofrinho.core.Abstractions;
using cofrinho.core.Enums;
using cofrinho.core.ValueObjects;
using MediatR;

namespace cofrinho.application.Commands.Objetivos.Editar;

public record EditarObjetivoByIdCommand(
    Guid Id,
    string? Titulo,
    string? Descricao,
    decimal? ValorAlvo,
    TipoMoedaEnum? TipoMoeda,
    DateTime? Prazo,
    StatusObjetivoEnum? Status)
    : IRequest<ResultViewModel>
{
    [JsonIgnore]
    public Guid Id { get; init; }
}

internal class EditarObjetivoByIdCommandHandler : IRequestHandler<EditarObjetivoByIdCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public EditarObjetivoByIdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultViewModel> Handle(EditarObjetivoByIdCommand request, CancellationToken cancellationToken)
    {
        var propriedadesAlteradas = new Dictionary<string, string>();

        var objetivo = await _unitOfWork.Objetivos.GetByIdAsync(request.Id, cancellationToken);
        
        if(objetivo is null)
            return ResultViewModel.Error("Não foi possível encontrar o objetivo.");
        
        if (!string.IsNullOrWhiteSpace(request.Titulo))
        {
            objetivo.AlterarTitulo(request.Titulo);
            propriedadesAlteradas.Add("Titulo", request.Titulo);
        }
        
        if (!string.IsNullOrWhiteSpace(request.Descricao))
        {
            objetivo.AlterarDescricao(request.Descricao);
            propriedadesAlteradas.Add("Descricao", request.Descricao);
        }
        
        if (request.ValorAlvo is not null)
        {
            if(request.TipoMoeda is null)
                objetivo.AddNotification("TipoMoeda", "Quando envia o valor alvo o tipo moeda não pode ser nulo.");

            objetivo.AlterarValorAlvo(request.ValorAlvo.Value, request.TipoMoeda.Value);
            propriedadesAlteradas.Add("ValorAlvo", request.ValorAlvo.ToString());
        }
        
        if (request.Prazo is not null)
        {
            objetivo.AlterarPrazo(request.Prazo.Value);
            propriedadesAlteradas.Add("Prazo", request.Prazo.Value.ToString());
        }

        if (request.Status is not null)
        {
            objetivo.AlterarStatus(request.Status.Value);
            propriedadesAlteradas.Add("Status", request.Status.Value.ToString());
        }
        
        if(!objetivo.IsValid)
            return ResultViewModel.ErrorsFromNotifications(objetivo.Notifications);
        
        await _unitOfWork.Objetivos.UpdateAsync(objetivo, cancellationToken);
        await _unitOfWork.CommitAsync();
        
        var mensagem = $"Propriedades alteradas: {string.Join(", ", propriedadesAlteradas.Keys)}";
        
        return ResultViewModel.Success(mensagem);
    }
}