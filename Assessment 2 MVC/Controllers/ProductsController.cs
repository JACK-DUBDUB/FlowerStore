using Assessment_2_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assessment_2_MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsController(ShopContext shopContext)
        {
            _shopContext = shopContext;
            _shopContext.Database.EnsureCreated();
        }


        // GET ALL PRODUCTS
        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery] ProductParametersQuery queryParameters)
        {
            IQueryable<Product> products =_shopContext.Products;


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

            // Search by Term
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products.Where(
                    p => p.Name.Trim().ToLower().Contains(
                        queryParameters.Name.ToLower()));
            }

            products = products.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);


            return Ok(await products.ToArrayAsync());
        }


        // GET PRODUCT BY ID
        [Route("api/[controller]")]
        [HttpGet]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);

            if (product == null) 
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult>PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_shopContext.Products.Any(p => p.Id == id))
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

        /*
        [HttpGet (Name = "GetAllProducts")]

        public IEnumerable<Product> GetProducts()
        {
            return _shopContext.Products.ToArray();
        }*/
    }
}
