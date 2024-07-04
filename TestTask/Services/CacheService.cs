using TestTask.Models;

namespace TestTask.Services
{
    public class CacheService
    {
        private readonly Dictionary<Guid, Models.Route> _cache = new();

        public void AddToCache(Models.Route route)
        {
            _cache[route.Id] = route;
        }

        public Models.Route[] SearchFromCache(SearchRequest request)
        {
            return _cache.Values.Where(r =>
                r.Origin == request.Origin &&
                r.Destination == request.Destination &&
                r.OriginDateTime >= request.OriginDateTime &&
                (request.Filters?.DestinationDateTime == null || r.DestinationDateTime <= request.Filters.DestinationDateTime) &&
                (request.Filters?.MaxPrice == null || r.Price <= request.Filters.MaxPrice) &&
                (request.Filters?.MinTimeLimit == null || r.TimeLimit >= request.Filters.MinTimeLimit)
            ).ToArray();
        }
    }
}

