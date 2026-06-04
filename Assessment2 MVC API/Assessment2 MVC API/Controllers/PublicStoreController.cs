using Assessment2_MVC_API.Data;
using Assessment2_MVC_API.Dtos;
using Assessment2_MVC_API.Models;
using Assessment2_MVC_API.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

// CURRENT MONGO SERVER PASSWORD: [REDACTED-MONGO-PASS]

namespace Assessment2_MVC_API.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PublicStoreControllerV2 : ControllerBase
    {
        private readonly MongoDbService _mongoDbService; // create instance of mongodbservice - enables interaction with mongodb server

        public PublicStoreControllerV2(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpGet("public_display_all_products")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAllProducts()
        {
            var productCollection = _mongoDbService.GetProductCollection();

            var products = await productCollection.Find(_ => true).ToListAsync();

            var p = new List<Product>();

            foreach (var item in products)
            {
                if (item.IsAvailable == true)
                {
                    p.Add(item);
                }
            }

            var productDisplay = p.Select(product => new
            {
                product.Id,
                product.Name,
                product.StoreLocation,
                product.PostCode,
                product.Price,
                product.IsAvailable,
                product.CategoryId,
            }).ToList();

            return Ok(productDisplay);
        }

        // This one is only for the header
        [HttpGet("public_display_all_products2")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAllProducts2()
        {

            var productCollection = _mongoDbService.GetProductCollection();

            var products = await productCollection.Find(_ => true).ToListAsync();

            var p = new List<Product>();

            foreach (var item in products)
            {
                if (item.IsAvailable == true)
                {
                    p.Add(item);
                }
            }

            var productDisplay = p.Select(product => new
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
    }

    [Authorize(Policy = "RequireAdminRole")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]

    public class PublicStoreControllerV1 : ControllerBase
    {
        private readonly MongoDbService _mongoDbService; // create instance of mongodbservice - enables interaction with mongodb server
        private readonly LocalDataService _localDataService; // create instance of localdataservice - collects seeded data from the local store context
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public PublicStoreControllerV1(MongoDbService mongoDbService, 
            LocalDataService localDataService, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration config)
        {
            _localDataService = localDataService;
            _mongoDbService = mongoDbService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        #region PUBLIC ACCESS
        // Collect all products
        [HttpGet("public_display_all_products")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAllProducts()
        {
            var productCollection = _mongoDbService.GetProductCollection();
            //var categoryCollection = _mongoDbService.GetCategoryCollection(); // <--

            var products = await productCollection.Find(_ => true).ToListAsync();
            //var categories = await categoryCollection.Find(_ => true).ToListAsync(); // <--

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
        [AllowAnonymous]
        public async Task<ActionResult> GetAllCategories()
        {
            var categoryCollection = _mongoDbService.GetCategoryCollection();
            var categories = await categoryCollection.Find(_ => true).ToListAsync();
            return Ok(categories);
        }

        // Search product by filter
        [HttpGet("public_display_filtered_products")]
        [AllowAnonymous]
        public async Task<ActionResult> GetFilteredProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            var productCollection = _mongoDbService.GetProductCollection();
            var products = await productCollection.Find(_ => true).ToListAsync();
            
            var filteredProducts = new List<Product>();

            // My version of a filter fuck AI
            foreach (var product in products)
            {
                bool matchesFilter = true;

                // Filter by Min + Max price parameter
                if ((queryParameters.MinPrice.HasValue && product.Price < queryParameters.MinPrice.Value) || (queryParameters.MaxPrice.HasValue && product.Price > queryParameters.MaxPrice.Value))
                {
                    matchesFilter = false;
                }

                 // Filter by CategoryId
                if (queryParameters.CategoryId.HasValue) 
                { 
                    if (product.CategoryId != queryParameters.CategoryId.Value) 
                    { 
                        matchesFilter = false; 
                    } 
                } 

                // Filter by Name (case-insensitive)
                if (!string.IsNullOrEmpty(queryParameters.Name)) 
                { 
                    if (!product.Name.Contains(queryParameters.Name, StringComparison.OrdinalIgnoreCase)) 
                    { 
                        matchesFilter = false; 
                    } 
                } 

                // Add product to filtered list if all conditions are met
                if (matchesFilter) 
                {
                    filteredProducts.Add(product); 
                } 
            }

            // Sort by type - this required help from AI - i just wanted it more concise for this part
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null) // HAS TO BE CASE SENSITIVE
                {
                    {
                        if (queryParameters.SortOrder.ToLower() == "desc") // Descending order
                        {
                            filteredProducts = filteredProducts.OrderByDescending(p => typeof(Product).GetProperty(queryParameters.SortBy).GetValue(p)).ToList();
                        }
                        else if (queryParameters.SortOrder.ToLower() == "asc") // Ascending order  
                        {
                            filteredProducts = filteredProducts.OrderBy(p => typeof(Product).GetProperty(queryParameters.SortBy).GetValue(p)).ToList();
                        }
                    }
                }
            }

            // If there are no products then return bad
            if (filteredProducts.Count == 0)
            {
                return NotFound("No products found matching the criteria.");
            }

            return Ok(filteredProducts);
        }

        [HttpGet("public_display_searched_products")]
        [AllowAnonymous]
        public async Task<ActionResult> GetProductSearch([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return BadRequest("Search term cannot be empty.");
                }

                var productCollection = _mongoDbService.GetProductCollection();

                // Perform the search
                var results = await productCollection.Find(product =>
                    product.Name.ToLower().Contains(searchTerm.ToLower()))
                    .ToListAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while searching products: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// ACCOUNTS ///
        // Create user account
        [HttpPost("public_register")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(appUser, user.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
            else
            {
                await _userManager.AddToRoleAsync(appUser, "User"); // Assign default role to newly registered account
            }

            return Ok("User created successfully");
        }

        // Log into account - All Information from: https://youtu.be/2R4RW7WaIWQ https://www.youtube.com/watch?v=w8I32UPEvj8&t=324s
        [HttpPost("public_login")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] Dtos.LoginRequest request)
        {
            var result = await LoginAsync(request);
            if (result.Success) { return new JsonResult(result) { StatusCode = (int)HttpStatusCode.OK }; }
            return new JsonResult(result.Message) { StatusCode = (int)HttpStatusCode.BadRequest };
        }

        private async Task<LoginResponse> LoginAsync(Dtos.LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) return new LoginResponse { Message = "Invalid email.", Success = false }; // Ideally i give a vague response but this helps me by knowing what went wrong in swagger

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded) return new LoginResponse { Message = "Invalid password.", Success = false };

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtConfig:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(60);

            var token = new JwtSecurityToken(
                issuer: _config["JwtConfig:Issuer"],
                audience: _config["JwtConfig:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new LoginResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Message = "Login Successful",
                Email = user?.Email,
                Success = true,
                ExpiresIn = (int)(token.ValidTo - DateTime.UtcNow).TotalSeconds // Calculate the remaining time in seconds - I gpt'd that line because i had no idea how to convert.
            };
        }

        // Edit user account - Not necessary
        // [Authorize(Policy = "RequireUserRole")]
        // Optional

        // Delete own user account - - Not necessary
        // [Authorize(Policy = "RequireUserRole")]
        // Optional

        #endregion

        #region ADMIN ACCESS ONLY
        // Add all local seed categories
        [HttpPost("admin_transfer_local_categories")]
        public async Task<IActionResult> TransferCategories()
        {
            var categories = _localDataService.GetSeededCategories();
            await _mongoDbService.InsertCategoriesAsync(categories);
            return Ok("Categories transferred successfully");
        }

        // Add all local seed products
        [HttpPost("admin_transfer_local_products")]
        public async Task<IActionResult> TransferProducts()
        {
            var products = _localDataService.GetSeededProducts();
            await _mongoDbService.InsertProductsAsync(products);
            return Ok("Products transferred successfully");

        }

        // Add new product to DB
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

        // Create Role - this isnt necessary as there should only be 2 roles for this assessment
        [HttpPost("admin_create_new_role")]
        public async Task<IActionResult> CreateRole([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = name });
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            return Ok("Role Created Successfully");
        }


        // Assign role to user accounts by Id
        [HttpPost("admin_switch_user_role")]
        public async Task<IActionResult> AssignNewRole([Required] string email, [Required] string newRole)
        {
            // Check if account exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the role exists
            if (!await _roleManager.RoleExistsAsync(newRole)) 
            { 
                return BadRequest("Role does not exist."); 
            }

            var currentRoles = await _userManager.GetRolesAsync(user); // get current role from user

            if (currentRoles.Count > 0) // if the user has a current role
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles); // Remove current roles from user 
                if (!removeRolesResult.Succeeded) 
                { 
                    return BadRequest("Failed to remove user from current roles."); 
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, newRole); // Add new role to user 
            if (!addRoleResult.Succeeded) 
            { 
                return BadRequest("Failed to assign new role to user."); 
            }

            return Ok($"User role switched to '{newRole}' successfully.");
        }

        // Delete user accounts - Not necessary


        #endregion
    }
}