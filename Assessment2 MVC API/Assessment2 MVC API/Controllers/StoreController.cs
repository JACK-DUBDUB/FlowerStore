using Assessment2_MVC_API.Models;
using Assessment2_MVC_API.Models.Extensions;
using Assessment2_MVC_API.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assessment2_MVC_API.Controllers
{
    // CURRENT MONGO SERVER PASSWORD: [REDACTED-MONGO-PASS]

    [Authorize]
    [ApiVersion("1.0")]
    [Route("products")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly StoreContext _storeContext;

        public StoreController(StoreContext storeContext)
        {
            _storeContext = storeContext;
            _storeContext.Database.EnsureCreated();
        }

        // CREATE
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // PERFORM CHECKS TO SEE IF VALID PRODUCT
            // CHECK ID, CHECK CATEGORY ETC

            // THEN ADD
            _storeContext.Products.Add(product);
            await _storeContext.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [AllowAnonymous]
        // GET ALL PRODUCTS
        [HttpGet("display_products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _storeContext.Products.ToListAsync();
            return Ok(products);
        }

        [AllowAnonymous]
        // GET CATEGORIES
        [HttpGet("display_categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _storeContext.Categories.ToListAsync();
            return Ok(categories);
        }

        [AllowAnonymous]
        // GET QUERIED PRODUCTS
        [HttpGet("display_by_query")]
        public async Task<ActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _storeContext.Products;

            // Min price parameter
            if (queryParameters.MinPrice != null)
            {
                products = products.Where(
                    p => p.Price >= queryParameters.MinPrice.Value);
            }
            // Max price parameter
            if (queryParameters.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price <= queryParameters.MaxPrice.Value);
            }

            if (queryParameters.CategoryId != null)
            {
                products = products.Where(p => p.CategoryId == queryParameters.CategoryId.Value);
            }

            // Search by SearchTerm
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                // Reassign the result back to products
                products = products.Where(
                    p => p.Name.Contains(
                        queryParameters.Name, StringComparison.CurrentCultureIgnoreCase));
            }

            // Sort results by desc/asc orders
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            products = products.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }

        [AllowAnonymous]
        // GET PRODUCT BY ID
        [Route("api/[controller]")]
        [HttpGet]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _storeContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }


        // UPDATE
        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            try
            {
                await _storeContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_storeContext.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _storeContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(); // 404 response
            }
            else
            {
                _storeContext.Products.Remove(product); // remove from shopcontext
                await _storeContext.SaveChangesAsync(); // save changes to local db

                return product;
            }
        }



    }
}
