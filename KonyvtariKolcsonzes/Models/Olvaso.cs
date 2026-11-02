using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KonyvtariKolcsonzes.Models;

public class Olvaso
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Nev { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(100)] public string Email { get; set; } = string.Empty;
    public List<Konyv> KonyvLista { get; set; } = new();
}
