using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CegAutoFlottaNyilv.Data;
using CegAutoFlottaNyilv.Models;

namespace CegAutoFlottaNyilv.Controllers;

[ApiController]
[Route("api/autok")]
public class AutoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await db.Auto.ToListAsync());

    [HttpGet("kereses")]
    public async Task<IActionResult> Search([FromQuery] string? szoveg)
    {
        if (string.IsNullOrWhiteSpace(szoveg)) return BadRequest();
        return Ok(await db.Auto.Where(x => x.Marka.Contains(szoveg) || x.Modell.Contains(szoveg) || x.Rendszam.Contains(szoveg)).ToListAsync());
    }

    [HttpGet("kiosztatlan")]
    public async Task<IActionResult> Free() => Ok(await db.Auto.Where(x => x.EmberId == null).ToListAsync());

    [HttpGet("{id:int}")] 
    public async Task<IActionResult> Get(int id)
    {
        var item = await db.Auto.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Auto item)
    {
        if (!ModelState.IsValid || !Valid(item)) return BadRequest();
        if (await db.Auto.AnyAsync(x => x.Rendszam == item.Rendszam)) return BadRequest("Az azonosító már létezik.");
        if (item.EmberId is not null && !await db.Ember.AnyAsync(x => x.Id == item.EmberId)) return BadRequest("Nem létező Ember.");
        item.Id = 0;
        db.Auto.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")] 
    public async Task<IActionResult> Put(int id, Auto item)
    {
        var current = await db.Auto.FindAsync(id);
        if (current is null) return NotFound();
        if (!ModelState.IsValid || !Valid(item)) return BadRequest();
        if (await db.Auto.AnyAsync(x => x.Id != id && x.Rendszam == item.Rendszam)) return BadRequest("Az azonosító már létezik.");
        if (item.EmberId is not null && !await db.Ember.AnyAsync(x => x.Id == item.EmberId)) return BadRequest("Nem létező Ember.");

        current.Marka = item.Marka;
        current.Modell = item.Modell;
        current.Rendszam = item.Rendszam;
        current.Evjarat = item.Evjarat;
        current.EmberId = item.EmberId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Auto.FindAsync(id);
        if (item is null) return NotFound();
        db.Auto.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static bool Valid(Auto x) => !string.IsNullOrWhiteSpace(x.Marka) && !string.IsNullOrWhiteSpace(x.Modell) && !string.IsNullOrWhiteSpace(x.Rendszam) && x.Evjarat >= 1950 && x.Evjarat <= 2100;
}
