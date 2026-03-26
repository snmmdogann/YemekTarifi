using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YemekTarifiApi.Models
{
    public class Puan
    {
        [Key]
        public int PuanId { get; set; }

        [ForeignKey("Tarif")]
        public int TarifId { get; set; }

        [ForeignKey("Kullanici")]
        public int KullaniciId { get; set; }

        public int Deger { get; set; } // 1-5 arası puan

        public string Yorum { get; set; }

        public Tarif Tarif { get; set; }

        public Kullanici Kullanici { get; set; }
        public DateTime EklenmeTarihi { get; set; }=DateTime.Now;
        public DateTime GuncellenmeTarihi { get; set; } = DateTime.Now;
    }
}
