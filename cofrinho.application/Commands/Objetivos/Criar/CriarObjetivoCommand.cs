using cofrinho.application.Models;
using cofrinho.core.Abstractions;
using cofrinho.core.Entities;
using cofrinho.core.Enums;
using MediatR;

namespace cofrinho.application.Commands.Objetivos.Criar;

public class CriarObjetivoCommand : IRequest<ResultViewModel<Guid>>
{
    public string Titulo { get; init; }
    public string Descricao { get; init; }
    public decimal? ValorAlvo { get; init; }
    public TipoMoedaEnum? TipoMoeda { get; init; }
    public DateTime? Prazo { get; init; }
    public CategoriaEnum Categoria { get; init; }
}

internal class CriarObjetivoCommandHandler : IRequestHandler<CriarObjetivoCommand, ResultViewModel<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CriarObjetivoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultViewModel<Guid>> Handle(CriarObjetivoCommand request, CancellationToken cancellationToken)
    {
        var objetivo = Objetivo.Criar(request.Titulo, request.Descricao, request.ValorAlvo, request.TipoMoeda,
            request.Prazo, request.Categoria);

        if (!objetivo.IsValid)
            return ResultViewModel<Guid>.ErrorsFromNotifications(objetivo.Notifications);

        await _unitOfWork.Objetivos.AddAsync(objetivo);
        await _unitOfWork.CommitAsync();


        return ResultViewModel<Guid>.Success(objetivo.Id, "Objetivo criado com sucesso.");
    }
}