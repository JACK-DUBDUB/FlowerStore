using System.Text.Json.Serialization;

namespace Assessment_2_MVC.Models
{
    public class Product
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string StoreLocation { get; set; }
        public int PostCode { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable {  get; set; }
        public int CategoryId { get; set; }

        // there is no sku

        [JsonIgnore]
        public virtual Category Category {  get; set; } 

    }
}
