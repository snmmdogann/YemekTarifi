using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class KategoriModel
    {
        public int KategoriId { get; set; }
        public string Ad { get; set; }
        public List<TarifKategori> TarifKategoriler { get; set; } = new List<TarifKategori>();
    }
}
