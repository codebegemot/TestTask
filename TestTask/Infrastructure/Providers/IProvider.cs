using TestTask.Models;

namespace TestTask.Infrastructure.Providers
{
	public interface IProvider
	{
        Task<ProviderRoute[]> SearchRoutesAsync(SearchRequest request, CancellationToken cancellationToken);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
    }
}

