using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YemekTarifiApi.Models
{
    public class Favori
    {
        [Key]
        public int FavoriId { get; set; }

        [ForeignKey("Kullanici")]
        public int KullaniciId { get; set; }

        [ForeignKey("Tarif")]
        public int TarifId { get; set; }
        public DateTime EklenmeTarihi { get; set; }= DateTime.Now;

        public Kullanici Kullanici { get; set; }
        public Tarif Tarif { get; set; }
    }
}
