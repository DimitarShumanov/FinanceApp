using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            return category;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            if (category == null)
                return BadRequest("Category is empty!");

            if (string.IsNullOrEmpty(category.Name))
                return BadRequest("Category Name is required!");

            if (string.IsNullOrEmpty(category.UserId))
                return BadRequest("UserId is required!");

            var exists = await _context.Categories.AnyAsync(c => c.Name == category.Name && c.UserId == category.UserId);

            if (exists)
            {
                return BadRequest($"Category \"{category}\" already exists!");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] string newName, [FromBody] bool isIncome)
        {
            if (string.IsNullOrEmpty(newName))
                return BadRequest("Category Name is required!");

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound("Category not found!");

            category.Name = newName;
            category.IsIncome = isIncome;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
