using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CegAutoFlottaNyilv.Models;

public class Ember
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Nev { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Osztaly { get; set; } = string.Empty;
    public List<Auto> AutoLista { get; set; } = new();
}
