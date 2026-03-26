using System.ComponentModel.DataAnnotations;

namespace YemekTarifiApi.Models
{
    public class Kategori
    {
        [Key]
        public int KategoriId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Ad { get; set; }
    }
}
