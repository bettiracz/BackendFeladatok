using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KonyvtariKolcsonzes.Data;
using KonyvtariKolcsonzes.Models;

namespace KonyvtariKolcsonzes.Controllers;

[ApiController]
[Route("api/konyvek")]
public class KonyvController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await db.Konyv.ToListAsync());

    [HttpGet("kereses")]
    public async Task<IActionResult> Search([FromQuery] string? szoveg)
    {
        if (string.IsNullOrWhiteSpace(szoveg)) return BadRequest();
        return Ok(await db.Konyv.Where(x => x.Cim.Contains(szoveg) || x.Szerzo.Contains(szoveg)).ToListAsync());
    }

    [HttpGet("szabad")]
    public async Task<IActionResult> Free() => Ok(await db.Konyv.Where(x => x.OlvasoId == null).ToListAsync());

    [HttpGet("{id:int}")] 
    public async Task<IActionResult> Get(int id)
    {
        var item = await db.Konyv.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Konyv item)
    {
        if (!ModelState.IsValid || !Valid(item)) return BadRequest();
        if (await db.Konyv.AnyAsync(x => x.ISBN == item.ISBN)) return BadRequest("Az azonosító már létezik.");
        if (item.OlvasoId is not null && !await db.Olvaso.AnyAsync(x => x.Id == item.OlvasoId)) return BadRequest("Nem létező Olvaso.");
        item.Id = 0;
        db.Konyv.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")] 
    public async Task<IActionResult> Put(int id, Konyv item)
    {
        var current = await db.Konyv.FindAsync(id);
        if (current is null) return NotFound();
        if (!ModelState.IsValid || !Valid(item)) return BadRequest();
        if (await db.Konyv.AnyAsync(x => x.Id != id && x.ISBN == item.ISBN)) return BadRequest("Az azonosító már létezik.");
        if (item.OlvasoId is not null && !await db.Olvaso.AnyAsync(x => x.Id == item.OlvasoId)) return BadRequest("Nem létező Olvaso.");
        if (current.OlvasoId is not null && item.OlvasoId != current.OlvasoId && item.OlvasoId is not null) return BadRequest("A könyvet előbb vissza kell hozni.");
        current.Cim = item.Cim;
        current.Szerzo = item.Szerzo;
        current.ISBN = item.ISBN;
        current.KiadasEve = item.KiadasEve;
        current.OlvasoId = item.OlvasoId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Konyv.FindAsync(id);
        if (item is null) return NotFound();
        db.Konyv.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static bool Valid(Konyv x) => !string.IsNullOrWhiteSpace(x.Cim) && !string.IsNullOrWhiteSpace(x.Szerzo) && !string.IsNullOrWhiteSpace(x.ISBN);
}
