using System.ComponentModel.DataAnnotations;

namespace YemekTarifiApi.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciId { get; set; }

        [Required]
        [MaxLength(100)]
        public string KullaniciAdi { get; set; }

        [MaxLength(100)]
        public string KullaniciSoyadi { get; set; }

        [Required]
        [MaxLength(100)]
        public string Sifre { get; set; }

        [MaxLength(50)]
        public string? Cinsiyet { get; set; }

        public DateTime? DogumTarihi { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Eposta { get; set; }

        [MaxLength(20)]
        public string? Telefon { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.Now;
    }
}
