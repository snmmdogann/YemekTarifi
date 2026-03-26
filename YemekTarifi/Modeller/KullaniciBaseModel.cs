using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class KullaniciBaseModel
    {
        public string KullaniciAdi { get; set; }
        public string Eposta { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
    }
}
