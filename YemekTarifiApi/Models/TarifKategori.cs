using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YemekTarifiApi.Models
{
    public class TarifKategori
    {
        [Key]
        public int TarifKategoriId { get; set; }

        [ForeignKey("Tarif")]
        public int TarifId { get; set; }

        [ForeignKey("Kategori")]
        public int KategoriId { get; set; }
    }
}
