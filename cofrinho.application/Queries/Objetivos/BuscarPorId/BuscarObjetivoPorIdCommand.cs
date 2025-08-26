using cofrinho.application.Models;
using cofrinho.application.Models.Objetivos;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Specifications;
using MapsterMapper;
using MediatR;

namespace cofrinho.application.Queries.Objetivos.BuscarPorId;

public record BuscarObjetivoPorIdCommand(Guid Id) : IRequest<ResultViewModel<ObjetivoComTransacoesViewModel>>;

internal class BuscarObjetivoPorIdCommandHandler : IRequestHandler<BuscarObjetivoPorIdCommand, ResultViewModel<ObjetivoComTransacoesViewModel>>
{
    private readonly IObjetivoRepository _objetivoRepository;

    public BuscarObjetivoPorIdCommandHandler(IObjetivoRepository objetivoRepository, IMapper mapper)
    {
        _objetivoRepository = objetivoRepository;
    }

    public async Task<ResultViewModel<ObjetivoComTransacoesViewModel>> Handle(BuscarObjetivoPorIdCommand request, CancellationToken cancellationToken)
    {
        var spec = new BuscarObjetivoComTransacoesSpec(request.Id);
        var objetivo = await _objetivoRepository.SingleOrDefaultAsync(spec, cancellationToken);

        if (objetivo is null)
            return ResultViewModel<ObjetivoComTransacoesViewModel>.Error("Não foi possível encontrar o objetivo.");
        
        
        var dto = ObjetivoComTransacoesViewModel.FromEntity(objetivo);
        
        return  ResultViewModel<ObjetivoComTransacoesViewModel>.Success(dto);
        
        
        
    }
}