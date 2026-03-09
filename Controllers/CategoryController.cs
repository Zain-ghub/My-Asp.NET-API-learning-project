using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepoApi.Data;
using RepoApi.Models;

namespace RepoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly RepoContext _context;

        public CategoryController(RepoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return Ok(await _context.Categories.ToListAsync());
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.Include(c=>c.Products).FirstOrDefaultAsync(c=> c.Id == id);

            if (category == null)  return NotFound();
            

            return Ok(category);
        }
        [HttpPost]
        public async Task<ActionResult<Category>> CategoryPost(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Category>> CategoryPut(int id, [FromBody] Category category)
        {
            if (id != category.Id) { return BadRequest(); }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Category>> CategoryDelete(int id)
        {
            var cat = await _context.Categories.FindAsync(id);

            if (cat == null) { return NotFound(); };

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
