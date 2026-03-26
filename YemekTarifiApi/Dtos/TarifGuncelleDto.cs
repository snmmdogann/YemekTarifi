using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace YemekTarifiApi.Dtos

{
    public class TarifGuncelleDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Ad { get; set; }

        [Required]
        public string Malzemeler { get; set; }

        [Required]
        public string Yapilis { get; set; }

        [Required]
        public int PisirmeSuresi { get; set; }
        public List<int> KategoriIdler { get; set; } = new();
        public IFormFile? Resim { get; set; }
    }
}