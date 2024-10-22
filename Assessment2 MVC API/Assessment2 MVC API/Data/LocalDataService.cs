using Assessment2_MVC_API.Models;

namespace Assessment2_MVC_API.Data
{
    public class LocalDataService
    {
        private readonly StoreContext _context;

        public LocalDataService(StoreContext context)
        {
            _context = context;
        }

        // Collect all products from the storecontext
        public List<Product> GetSeededProducts()
        {
            return _context.Products.ToList();
        }

        // collect all categories from store context
        public List<Category> GetSeededCategories()
        {
            return _context.Categories.ToList();
        }
    }
}
