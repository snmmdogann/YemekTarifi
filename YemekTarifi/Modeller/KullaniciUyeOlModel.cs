using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class KullaniciUyeOlModel:KullaniciBaseModel
    {
        public string KullaniciSoyadi { get; set; }
        public string Sifre { get; set; }
        public string SifreTekrar { get; set; }
        public string? Cinsiyet { get; set; }
        public string? DogumTarihi { get; set; }
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        public string Telefon { get; set; }


    }
}

