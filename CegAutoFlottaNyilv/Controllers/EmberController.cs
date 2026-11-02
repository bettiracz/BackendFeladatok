using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CegAutoFlottaNyilv.Data;
using CegAutoFlottaNyilv.Models;

namespace CegAutoFlottaNyilv.Controllers;

[ApiController]
[Route("api/emberek")]
public class EmberController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await db.Ember.Include(x => x.AutoLista).ToListAsync());

    [HttpGet("{id:int}")] 
    public async Task<IActionResult> Get(int id)
    {
        var item = await db.Ember.Include(x => x.AutoLista).FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{id:int}/autok")]
    public async Task<IActionResult> GetRelated(int id)
    {
        if (!await db.Ember.AnyAsync(x => x.Id == id)) return NotFound();
        return Ok(await db.Auto.Where(x => x.EmberId == id).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Post(Ember item)
    {
        if (!ModelState.IsValid || !Valid(item)) return BadRequest();
        item.Id = 0;
        item.AutoLista = new();
        db.Ember.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")] 
    public async Task<IActionResult> Put(int id, Ember item)
    {
        var current = await db.Ember.FindAsync(id);
        if (current is null) return NotFound();
        if (!ModelState.IsValid || !Valid(item)) return BadRequest();
        current.Nev = item.Nev;
        current.Osztaly = item.Osztaly;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")] 
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Ember.FindAsync(id);
        if (item is null) return NotFound();
        db.Ember.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static bool Valid(Ember x) => !string.IsNullOrWhiteSpace(x.Nev) && !string.IsNullOrWhiteSpace(x.Osztaly);
}
