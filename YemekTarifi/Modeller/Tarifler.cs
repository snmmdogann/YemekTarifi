using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Servisler;
namespace YemekTarifi.Modeller
{
    public class Tarifler:TarifBaseModel
    {
        public int Id { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public int KategoriId { get; set; }
        public string? ResimUrl { get; set; }
        public List<PuanModel> Yorum { get; set; } = new List<PuanModel>();
        public List<TarifKategori> TarifKategoriler { get; set; } = new List<TarifKategori>();
        public string ResimYolu {  get; set; }

        public override string Hazirla()
        {
            return $" Tarif gösteriliyor: {Ad}";
        }
    }

}

