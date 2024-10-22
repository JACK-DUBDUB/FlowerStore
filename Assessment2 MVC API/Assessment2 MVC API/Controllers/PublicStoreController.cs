using Assessment2_MVC_API.Data;
using Assessment2_MVC_API.Models;
using Assessment2_MVC_API.Models.Extensions;
using Assessment2_MVC_API.Models.Queries;
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
        private readonly MongoDbService _mongoDbService;

        public PublicStoreController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        // Collect all products
        [HttpGet("display_all_products")]
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
                CategoryName = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name // i can get rid of this but the above needs to go
            }).ToList();

            return Ok(productDisplay);

        }

        // Collect all categories
        [HttpGet("display_categories")]
        public async Task<ActionResult> GetAllCategories()
        {
            var categoryCollection = _mongoDbService.GetCategoryCollection();
            var categories = await categoryCollection.Find(_ => true).ToListAsync();
            return Ok(categories);
        }


        // Search product by filter
        [HttpGet("display_filtered_products")]
        public async Task<ActionResult> GetFilteredProducts([FromQuery] ProductQueryParameters queryParameters)
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
    }
}