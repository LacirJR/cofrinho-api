using System.Text.Json.Serialization;
using cofrinho.application.Models;
using cofrinho.core.Abstractions;
using cofrinho.core.Entities;
using cofrinho.core.Enums;
using cofrinho.core.ValueObjects;
using MediatR;

namespace cofrinho.application.Commands.Transacoes.Criar;

public record CriarTransacoesCommand(
    decimal Valor,
    TipoMoedaEnum TipoMoeda,
    TipoTransacaoEnum TipoTransacao,
    DateTime? DataTransacao) : IRequest<ResultViewModel<Guid>>
{
    [JsonIgnore] public Guid ObjetivoId { get; init; }
}

internal class CriarTransacoesCommandHandler : IRequestHandler<CriarTransacoesCommand, ResultViewModel<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CriarTransacoesCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultViewModel<Guid>> Handle(CriarTransacoesCommand request, CancellationToken cancellationToken)
    {
        var transacao = Transacao.Criar(Dinheiro.Criar(request.Valor, request.TipoMoeda), request.TipoTransacao,
            request.DataTransacao ?? DateTime.UtcNow, request.ObjetivoId);

        if (!transacao.IsValid)
            return ResultViewModel<Guid>.ErrorsFromNotifications(transacao.Notifications);


        await _unitOfWork.Transacoes.AddAsync(transacao, cancellationToken);
        await _unitOfWork.CommitAsync();
        
        return ResultViewModel<Guid>.Success(transacao.Id);
    }
}