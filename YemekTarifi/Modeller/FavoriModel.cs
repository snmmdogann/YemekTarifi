using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class FavoriModel
    {
        public int KullaniciId { get; set; }
        public int TarifId { get; set; }
        public int KategoriId {  get; set; }
        public string KategoriAdi {  get; set; }
        
        public string KullaniciAdi {  get; set; }
        public string TarifAdi { get; set; }
        public DateTime EklenmeTarihi { get; set; }=DateTime.Now;
    }
}