namespace TestTask.Models
{
	public class SearchFilters
	{
        public DateTime? DestinationDateTime { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? MinTimeLimit { get; set; }
        public bool? OnlyCached { get; set; }
    }
}

