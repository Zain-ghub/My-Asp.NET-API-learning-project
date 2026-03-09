using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepoApi.Data;
using RepoApi.Models;

namespace RepoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
        public class BrandController : Controller
        {
            private readonly RepoContext _context;

            public BrandController(RepoContext context)
            {
                _context = context;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
            {
                return Ok(await _context.Brands.ToListAsync());
            }
            [HttpGet("{id:int}")]
            public async Task<ActionResult<Brand>> GetBrand(int id)
            {
                var brand = await _context.Brands.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);

                if (brand == null) return NotFound();


                return Ok(brand);
            }
            [HttpPost]
            public async Task<ActionResult<Brand>> PostBrand(Brand brand)
            {
                await _context.Brands.AddAsync(brand);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
            }
            [HttpPut("{id:int}")]
            public async Task<ActionResult<Brand>> PutBrand(int id, [FromBody] Brand brand)
            {
                if (id != brand.Id) { return BadRequest(); }

                _context.Entry(brand).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                      if (!await _context.Brands.AnyAsync(p => p.Id == id))
                      return NotFound();
                   throw;
                }
                return NoContent();

            }
            [HttpDelete("{id:int}")]
            public async Task<ActionResult<Brand>> DeleteBrand(int id)
            {
                var brand = await _context.Brands.FindAsync(id);

                if (brand == null) { return NotFound(); }
                ;

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }

