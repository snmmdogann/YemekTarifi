using System.ComponentModel.DataAnnotations.Schema;

namespace YemekTarifiApi.Models
{
    public class FavoriEkle
    {
        
        public int KullaniciId { get; set; }

        
        public int TarifId { get; set; }

        public DateTime EklenmeTarihi { get; set; }= DateTime.Now;
    }
}
