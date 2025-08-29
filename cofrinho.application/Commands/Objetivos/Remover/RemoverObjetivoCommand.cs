using System.Text.Json.Serialization;
using cofrinho.application.Models;
using cofrinho.core.Abstractions;
using MediatR;

namespace cofrinho.application.Commands.Objetivos.Remover;

public record RemoverObjetivoCommand(Guid Id) : IRequest<ResultViewModel>;

internal class RemoverObjetivoCommandHandler : IRequestHandler<RemoverObjetivoCommand, ResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoverObjetivoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultViewModel> Handle(RemoverObjetivoCommand request, CancellationToken cancellationToken)
    {
        var objetivo = await _unitOfWork.Objetivos.GetByIdAsync(request.Id, cancellationToken);

        if (objetivo is null)
            return ResultViewModel.Error("Objetivo não encontrado");

        objetivo.Deletar();

        await _unitOfWork.Objetivos.UpdateAsync(objetivo, cancellationToken);
        await _unitOfWork.CommitAsync();

        return ResultViewModel.Success("Objetivo removido com sucesso.");
    }
}