using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CegAutoFlottaNyilv.Models;

public class Auto
{
    public int Id { get; set; }
    [Required, MaxLength(50)] public string Marka { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string Modell { get; set; } = string.Empty;
    [Required, MaxLength(15)] public string Rendszam { get; set; } = string.Empty;
    [Range(1950, 2100)] public int Evjarat { get; set; }
    public int? EmberId { get; set; }
    [JsonIgnore] public Ember? Ember { get; set; }
}
