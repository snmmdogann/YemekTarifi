using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class TarifEkleDto
{
    [Required]
    public string Ad { get; set; }

    [Required]
    public string Malzemeler { get; set; }

    [Required]
    public string Yapilis { get; set; }

    [Required]
    public string EkleyenKullanici { get; set; }

    [Required]
    public int PisirmeSuresi { get; set; }

    [Required]
    public IFormFile Resim { get; set; }
}
