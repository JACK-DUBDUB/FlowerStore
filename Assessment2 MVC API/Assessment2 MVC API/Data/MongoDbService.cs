using Assessment2_MVC_API.Models;
using MongoDB.Driver;

// INFORMATION FROM:
// https://www.youtube.com/watch?v=Gxf7zBl5Z64
// https://www.youtube.com/watch?v=BfEjDD8mWYg
// https://youtu.be/exXavNOqaVo


namespace Assessment2_MVC_API.Data
{
    public class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;
        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("DbConnection");
            var mongoUrl = MongoUrl.Create(connectionString); // notvalid
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoDatabase? Database => _database;

        public IMongoCollection<Category> GetCategoryCollection()
        {
            return _database.GetCollection<Category>("categories");
        }

        public IMongoCollection<Product> GetProductCollection()
        {
            return _database.GetCollection<Product>("products");
        }

        // Collect all local categories and then send to DB
        public async Task InsertCategoriesAsync(List<Category> categories)
        {
            var categoryCollection = GetCategoryCollection();

            try
            {
                foreach (var category in categories)
                {
                    var exists = await categoryCollection.Find(c => c.Id == category.Id).AnyAsync();
                    if (!exists)
                    {
                        await categoryCollection.InsertOneAsync(category);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting categories: " + ex.Message, ex);
            }
        }

        // Collect all local products and send to DB
        public async Task InsertProductsAsync(List<Product> products)
        {
            var productCollection = GetProductCollection();

            try
            {
                foreach (var product in products)
                {
                    var exists = await productCollection.Find(p => p.Id == product.Id).AnyAsync();
                    if (!exists)
                    {
                        await productCollection.InsertOneAsync(product);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting products: " + ex.Message, ex);

            }
        }

        // Get newest product and send to DB


    }
}
