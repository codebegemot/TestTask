using TestTask.Models;

namespace TestTask.Infrastructure.Providers
{
	public class ProviderOne : IProvider
	{
        private readonly HttpClient _httpClient;

        public ProviderOne(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProviderRoute[]> SearchRoutesAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var providerRequest = new ProviderOneSearchRequest
            {
                From = request.Origin,
                To = request.Destination,
                DateFrom = request.OriginDateTime,
                DateTo = request.Filters?.DestinationDateTime,
                MaxPrice = request.Filters?.MaxPrice
            };

            var response = await _httpClient.PostAsJsonAsync("http://provider-one/api/v1/search", providerRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ProviderOneSearchResponse>(cancellationToken: cancellationToken);
            return result.Routes.Select(r => new ProviderRoute
            {
                Origin = r.From,
                Destination = r.To,
                OriginDateTime = r.DateFrom,
                DestinationDateTime = r.DateTo,
                Price = r.Price,
                TimeLimit = r.TimeLimit
            }).ToArray();
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("http://provider-one/api/v1/ping", cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
    public class ProviderOneSearchRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? MaxPrice { get; set; }
    }

    public class ProviderOneSearchResponse
    {
        public ProviderOneRoute[] Routes { get; set; }
    }

    public class ProviderOneRoute
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal Price { get; set; }
        public DateTime TimeLimit { get; set; }
    }
}

