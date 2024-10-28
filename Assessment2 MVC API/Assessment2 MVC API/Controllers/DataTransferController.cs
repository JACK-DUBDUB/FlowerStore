using Assessment2_MVC_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// This Controller connects to the database so that we can transfer local data to the 

namespace Assessment2_MVC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataTransferController : ControllerBase
    {
        private readonly LocalDataService _localDataService; // create instance of localdataservice - collects seeded data from the store context
        private readonly MongoDbService _mongoDbService; // create instance of mongodbservice - provides the connection to the mongodb server i have created

        public DataTransferController(LocalDataService localDataService, MongoDbService mongoDbService)
        {
            _localDataService = localDataService;
            _mongoDbService = mongoDbService;
        }

        // UNFORTUNATELY - I cannot actually process these 2 at the same time as of yet - but i dont think it matters too much TBH

        [HttpPost("admin_transfer-categories")]
        public async Task<IActionResult> TransferCategories()
        {
            var categories = _localDataService.GetSeededCategories();
            await _mongoDbService.InsertCategoriesAsync(categories);
            return Ok("Categories transferred successfully");
        }

        [HttpPost("admin_transfer-products")]
        public async Task<IActionResult> TransferProducts()
        {
            var products = _localDataService.GetSeededProducts();
            await _mongoDbService.InsertProductsAsync(products);
            return Ok("Products transferred successfully");

        }

        // I want to add a new product






    }
}
