using Assessment2_MVC_API.Data;
using Assessment2_MVC_API.Models;
using Assessment2_MVC_API.Models.Extensions;
using Assessment2_MVC_API.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Reflection;

namespace Assessment2_MVC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicStoreController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService; // create instance of mongodbservice - enables interaction with mongodb server
        private readonly LocalDataService _localDataService; // create instance of localdataservice - collects seeded data from the local store context

        public PublicStoreController(MongoDbService mongoDbService, LocalDataService localDataService)
        {
            _localDataService = localDataService;
            _mongoDbService = mongoDbService;
        }

        #region PUBLIC ACCESS
        // Collect all products
        [HttpGet("public_display_all_products")]
        public async Task<ActionResult> GetAllProducts()
        {
            var productCollection = _mongoDbService.GetProductCollection();
            var categoryCollection = _mongoDbService.GetCategoryCollection(); // <--

            var products = await productCollection.Find(_ => true).ToListAsync();
            var categories = await categoryCollection.Find(_ => true).ToListAsync(); // <--

            var productDisplay = products.Select(product => new
            {
                product.Id,
                product.Name,
                product.StoreLocation,
                product.PostCode,
                product.Price,
                product.IsAvailable,
                product.CategoryId,
                //CategoryName = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name // i can get rid of this but the above needs to go
            }).ToList();

            return Ok(productDisplay);

        }

        // Collect all categories
        [HttpGet("public_display_all_categories")]
        public async Task<ActionResult> GetAllCategories()
        {
            var categoryCollection = _mongoDbService.GetCategoryCollection();
            var categories = await categoryCollection.Find(_ => true).ToListAsync();
            return Ok(categories);
        }

        // Search product by filter
        [HttpGet("public_display_filtered_products")]
        public async Task<ActionResult> GetFilteredProducts([FromQuery] ProductQueryParameters queryParameters) // Yep this one is gpt'd and i don't care.
        {
            var productCollection = _mongoDbService.GetProductCollection();

            // Build the filter
            var filterBuilder = Builders<Product>.Filter;
            var filters = new List<FilterDefinition<Product>>();

            // Min price parameter
            if (queryParameters.MinPrice != null)
            {
                filters.Add(filterBuilder.Gte(p => p.Price, queryParameters.MinPrice.Value));
            }

            // Max price parameter
            if (queryParameters.MaxPrice != null)
            {
                filters.Add(filterBuilder.Lte(p => p.Price, queryParameters.MaxPrice.Value));
            }

            // Filter by CategoryId
            if (queryParameters.CategoryId != null)
            {
                filters.Add(filterBuilder.Eq(p => p.CategoryId, queryParameters.CategoryId.Value));
            }

            // Search by SearchTerm
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                filters.Add(filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(queryParameters.Name, "i")));
            }

            // Combine all filters
            var combinedFilter = filters.Count > 0 ? filterBuilder.And(filters) : FilterDefinition<Product>.Empty;

            // Sort results by desc/asc orders
            var sortBuilder = Builders<Product>.Sort;
            var sort = queryParameters.SortOrder.ToLower() == "desc" ? sortBuilder.Descending(queryParameters.SortBy) : sortBuilder.Ascending(queryParameters.SortBy);

            // Fetch filtered and sorted results with pagination
            var products = await productCollection.Find(combinedFilter)
                                                  .Sort(sort)
                                                  .Skip(queryParameters.Size * (queryParameters.Page - 1))
                                                  .Limit(queryParameters.Size)
                                                  .ToListAsync();

            return Ok(products);
        }

        /// ACCOUNTS ///
        // Create user account
        // TODO <--

        // Edit user account
        // TODO <--

        // Log into account
        // TODO <--

        // Delete own account
        // TODO <--

        #endregion

        #region ADMIN ACCESS
        // Add all local seed categories
        //[Authorize]
        [HttpPost("admin_transfer_local_categories")]
        public async Task<IActionResult> TransferCategories()
        {
            var categories = _localDataService.GetSeededCategories();
            await _mongoDbService.InsertCategoriesAsync(categories);
            return Ok("Categories transferred successfully");
        }

        // Add all local seed products
        //[Authorize]
        [HttpPost("admin_transfer_local_products")]
        public async Task<IActionResult> TransferProducts()
        {
            var products = _localDataService.GetSeededProducts();
            await _mongoDbService.InsertProductsAsync(products);
            return Ok("Products transferred successfully");

        }

        // Add new product to DB
        [Authorize]
        [HttpPost("admin_add_product")]
        public async Task<IActionResult> AddProduct([FromBody] Product newProduct)
        {
            if (newProduct == null)
            {
                return BadRequest("Product is null");
            }

            var productCollection = _mongoDbService.GetProductCollection();

            // Check if a product with the same ID already exists
            var existingProduct = await productCollection.Find(p => p.Id == newProduct.Id).FirstOrDefaultAsync();
            if (existingProduct != null)
            {
                return Conflict("Product with the same ID already exists");
            }

            // Insert the new product into MongoDB
            await productCollection.InsertOneAsync(newProduct);
            return Ok("Product added successfully");
        }

        // Edit selected product by index
        [Authorize]
        [HttpPut("admin_update_product/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                return BadRequest("Product is null");
            }

            var productCollection = _mongoDbService.GetProductCollection();

            // Find the product by ID and update
            var updateResult = await productCollection.ReplaceOneAsync(p => p.Id == id, updatedProduct);

            if (updateResult.MatchedCount == 0)
            {
                return NotFound($"Product with Id = {id} not found");
            }

            return Ok("Product updated successfully");
        }

        // Delete selected product by index
        [Authorize]
        [HttpDelete("admin_delete_product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productCollection = _mongoDbService.GetProductCollection();

            // Delete the product by ID
            var deleteResult = await productCollection.DeleteOneAsync(p => p.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return NotFound($"Product with Id = {id} not found");
            }

            return Ok("Product deleted successfully");
        }

        /// ACCOUNTS ///
        // Delete user accounts
        // TODO <--

        // Assign role to user accounts by Id
        // TODO <--

        #endregion
    }
}