using TestTask.Infrastructure.Providers;
using TestTask.Models;

namespace TestTask.Services
{
    public class SearchService : ISearchService
    {
        private readonly IEnumerable<IProvider> _providers;
        private readonly CacheService _cacheService;

        public SearchService(IEnumerable<IProvider> providers, CacheService cacheService)
        {
            _providers = providers;
            _cacheService = cacheService;
        }

        public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var routes = request.Filters?.OnlyCached == true
                ? _cacheService.SearchFromCache(request)
                : (await Task.WhenAll(_providers.Select(p => p.SearchRoutesAsync(request, cancellationToken))))
                    .SelectMany(r => r)
                    .Where(r => r.TimeLimit > DateTime.UtcNow)
                    .Select(r => new Models.Route
                    {
                        Id = Guid.NewGuid(),
                        Origin = r.Origin,
                        Destination = r.Destination,
                        OriginDateTime = r.OriginDateTime,
                        DestinationDateTime = r.DestinationDateTime,
                        Price = r.Price,
                        TimeLimit = r.TimeLimit
                    })
                    .ToArray();

            foreach (var route in routes)
            {
                _cacheService.AddToCache(route);
            }

            return new SearchResponse
            {
                Routes = routes,
                MinPrice = routes.Any() ? routes.Min(r => r.Price) : 0,
                MaxPrice = routes.Any() ? routes.Max(r => r.Price) : 0,
                MinMinutesRoute = routes.Any() ? (int)routes.Min(r => (r.DestinationDateTime - r.OriginDateTime).TotalMinutes) : 0,
                MaxMinutesRoute = routes.Any() ? (int)routes.Max(r => (r.DestinationDateTime - r.OriginDateTime).TotalMinutes) : 0,
            };
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            var statuses = await Task.WhenAll(_providers.Select(p => p.IsAvailableAsync(cancellationToken)));
            return statuses.All(status => status);
        }
    }
}

