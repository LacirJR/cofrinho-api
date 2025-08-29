using System.Threading;
using System.Threading.Tasks;

namespace cofrinho.core.Abstractions.Services;

public interface ITaxaRendimentoProvider
{
    Task<decimal> ObterTaxaAnualAsync(DateOnly data, CancellationToken cancellationToken = default);
}
