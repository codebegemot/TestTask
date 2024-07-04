using TestTask.Models;

namespace TestTask.Infrastructure.Providers
{
    public class ProviderTwo : IProvider
    {
        private readonly HttpClient _httpClient;

        public ProviderTwo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProviderRoute[]> SearchRoutesAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var providerRequest = new ProviderTwoSearchRequest
            {
                Departure = request.Origin,
                Arrival = request.Destination,
                DepartureDate = request.OriginDateTime,
                MinTimeLimit = request.Filters?.MinTimeLimit
            };

            var response = await _httpClient.PostAsJsonAsync("http://provider-two/api/v1/search", providerRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ProviderTwoSearchResponse>(cancellationToken: cancellationToken);
            return result.Routes.Select(r => new ProviderRoute
            {
                Origin = r.Departure.Point,
                Destination = r.Arrival.Point,
                OriginDateTime = r.Departure.Date,
                DestinationDateTime = r.Arrival.Date,
                Price = r.Price,
                TimeLimit = r.TimeLimit
            }).ToArray();
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("http://provider-two/api/v1/ping", cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }

    public class ProviderTwoSearchRequest
    {
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? MinTimeLimit { get; set; }
    }

    public class ProviderTwoSearchResponse
    {
        public ProviderTwoRoute[] Routes { get; set; }
    }

    public class ProviderTwoRoute
    {
        public ProviderTwoPoint Departure { get; set; }
        public ProviderTwoPoint Arrival { get; set; }
        public decimal Price { get; set; }
        public DateTime TimeLimit { get; set; }
    }

    public class ProviderTwoPoint
    {
        public string Point { get; set; }
        public DateTime Date { get; set; }
    }
}

