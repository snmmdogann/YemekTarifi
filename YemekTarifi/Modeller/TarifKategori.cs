using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class TarifKategori
    {
        public int TarifId { get; set; }
        public int KategoriId { get; set; }

        public Tarifler Tarif { get; set; }
        public KategoriModel Kategori { get; set; }
    }
}
