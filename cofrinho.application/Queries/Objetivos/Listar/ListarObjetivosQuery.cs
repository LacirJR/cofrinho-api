using Ardalis.Specification;
using cofrinho.application.Models;
using cofrinho.application.Models.Objetivos;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Specifications;
using MapsterMapper;
using MediatR;

namespace cofrinho.application.Queries.Objetivos.Listar;

public record ListarObjetivosQuery(int Page = 1, int PageSize = 10) : IRequest<ResultViewModel<PagedResult<ObjetivoViewModel>>>;

internal class ListarObjetivosQueryHandler : IRequestHandler<ListarObjetivosQuery, ResultViewModel<PagedResult<ObjetivoViewModel>>>
{
    private readonly IObjetivoRepository _objetivoRepository;
    private readonly IMapper _mapper;

    public ListarObjetivosQueryHandler(IObjetivoRepository objetivoRepository, IMapper mapper)
    {
        _objetivoRepository = objetivoRepository;
        _mapper = mapper;
    }

    public async Task<ResultViewModel<PagedResult<ObjetivoViewModel>>> Handle(ListarObjetivosQuery request, CancellationToken cancellationToken)
    {
        var spec = new ListagemObjetivosSpecification();

        var totalItems = await _objetivoRepository.CountAsync(spec, cancellationToken);
        
        spec.Query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);
        
        var objetivos = await _objetivoRepository.ListAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<ObjetivoViewModel>>(objetivos);
        
        var pagedResult = new PagedResult<ObjetivoViewModel>(dtos, totalItems, request.Page, request.PageSize);
        
        
        return ResultViewModel<PagedResult<ObjetivoViewModel>>.Success(pagedResult);
    }
}