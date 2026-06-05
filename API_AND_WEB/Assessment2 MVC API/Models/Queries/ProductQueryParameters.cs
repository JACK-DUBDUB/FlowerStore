namespace Assessment2_MVC_API.Models.Queries
{
    public class ProductQueryParameters : QueryParameters
    {
        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public int? CategoryId {  get; set; }

        public string Name { get; set; } = String.Empty;
    }
}
