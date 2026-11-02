using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KonyvtariKolcsonzes.Data;
using KonyvtariKolcsonzes.Models;

namespace KonyvtariKolcsonzes.Controllers;

[ApiController]
[Route("api/olvasok")]
public class OlvasoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await db.Olvaso.Include(x => x.KonyvLista).ToListAsync());

    [HttpGet("{id:int}")] 
    public async Task<IActionResult> Get(int id)
    {
        var item = await db.Olvaso.Include(x => x.KonyvLista).FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{id:int}/konyvek")]
    public async Task<IActionResult> GetRelated(int id)
    {
        if (!await db.Olvaso.AnyAsync(x => x.Id == id)) return NotFound();
        return Ok(await db.Konyv.Where(x => x.OlvasoId == id).ToListAsync());
    }
    [HttpGet("kolcsonzok")]
    public async Task<IActionResult> Borrowers() => Ok(await db.Olvaso.Include(x => x.KonyvLista).Where(x => x.KonyvLista.Any()).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Post(Olvaso item)
    {
        if (!ModelState.IsValid || !Valid(item)) return BadRequest();
        item.Id = 0;
        item.KonyvLista = new();
        db.Olvaso.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")] 
    public async Task<IActionResult> Put(int id, Olvaso item)
    {
        var current = await db.Olvaso.FindAsync(id);
        if (current is null) return NotFound();
        if (!ModelState.IsValid || !Valid(item)) return BadRequest();
        current.Nev = item.Nev;
        current.Email = item.Email;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Olvaso.FindAsync(id);
        if (item is null) return NotFound();
        db.Olvaso.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static bool Valid(Olvaso x) => !string.IsNullOrWhiteSpace(x.Nev) && !string.IsNullOrWhiteSpace(x.Email);
}
