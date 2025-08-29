using System.Threading;
using System.Threading.Tasks;

namespace cofrinho.core.Abstractions.Services;

public interface IDiasUteisService
{
    Task<bool> EhDiaUtil(DateOnly data, CancellationToken cancellationToken = default);
    Task<DateOnly> ProximoDiaUtil(DateOnly data, CancellationToken cancellationToken = default);
}
