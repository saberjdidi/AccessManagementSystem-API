using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;
using AccessManagementSystem_API.Services;
using AccessManagementSystem_API.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessManagementSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            List<Product> listProducts = new List<Product>();
            var result = await _service.GetAllProductsAsync();
            foreach (var product in result) { 
             Product productDto = new Product
             {
                 Id = product.Id,
                 Name = product.Name,   
                 Description = product.Description,
                 Category = product.Category,
                 CategoryId = product.CategoryId,
                 CreatedAt = DateTime.Now,
                 Price = product.Price,
                 ImagePath = $"https://localhost:7292/images/{product.ImagePath}",
             };
                listProducts.Add(productDto);
            }
            return Ok(listProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _service.GetProductByIdAsync(id);
                if (product is null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateProduct([FromBody] Product product)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    await _service.AddProductAsync(product);
        //    return Ok();
        //}

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                // Check if a file is provided
                if (productDto.File == null || productDto.File.Length == 0)
                    return BadRequest("Image is required.");

                // Step 1: Upload the file
                var directoryPath = "D:/App/Images";
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(productDto.File.FileName);
                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await productDto.File.CopyToAsync(stream);
                }

                // Step 2: Set the ImagePath in the Product model
                //product.ImagePath = fileName; // Save only the file name, not the full path
                // Map ProductDto to Product
                var product = new Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    CategoryId = productDto.CategoryId,
                    ImagePath = fileName // Save only the file name
                };

                // Step 3: Save the Product in the database
                await _service.AddProductAsync(product);

                return Ok(new { Message = "Product created successfully.", Product = product });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id || !ModelState.IsValid) return BadRequest();
            await _service.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _service.DeleteProductAsync(id);
            return NoContent();
        }

        //[HttpPost("UploadImage")]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("File is missing.");

        //    var directoryPath = "D:/App/Images";
        //    if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        //    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Generate unique name
        //    var filePath = Path.Combine(directoryPath, fileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    return Ok(new { FileName = fileName });
        //}
    }
}
