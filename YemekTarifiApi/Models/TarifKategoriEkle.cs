using System.ComponentModel.DataAnnotations.Schema;

namespace YemekTarifiApi.Models
{
    public class TarifKategoriEkle
    {
        
        public int TarifId { get; set; }

        public int KategoriId { get; set; }
    }
}
