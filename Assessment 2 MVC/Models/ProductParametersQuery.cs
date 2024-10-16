using Microsoft.AspNetCore.Routing.Constraints;

namespace Assessment_2_MVC.Models
{
    public class ProductParametersQuery : QueryParameters
    {
        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string Name { get; set; } = String.Empty;

        public string SearchTerm { get; set; } = String.Empty;

    }
}
