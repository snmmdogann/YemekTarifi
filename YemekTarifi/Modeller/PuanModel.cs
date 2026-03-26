using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class PuanModel
    {
        private int _deger;
        public int PuanId { get; set; }
        public int TarifId { get; set; }
        public string TarifAdi { get; set; }
        public int KullaniciId { get; set; }
        public string KullaniciAdi {  get; set; }
        public int Deger
        {
            get => _deger;
            set => _deger = value;
        }
        public DateTime Tarih { get; set; }
        public string Yorum { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public Tarifler Tarif { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public List<string> ValidationErrors { get; private set; } = new List<string>();
        public bool IsValid()
        {
            ValidationErrors.Clear();

            if (Deger < 1 || Deger > 5)
                ValidationErrors.Add("Puan 1 ile 5 arasında olmalıdır.");

            if (string.IsNullOrWhiteSpace(Yorum))
                ValidationErrors.Add("Yorum alanı boş olamaz.");

            return ValidationErrors.Count == 0;
        }

    }
}
