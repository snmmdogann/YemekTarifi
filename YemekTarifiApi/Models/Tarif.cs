using System.ComponentModel.DataAnnotations;

namespace YemekTarifiApi.Models
{
    public class Tarif
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Malzemeler { get; set; }
        public string Yapilis { get; set; }
        public int PisirmeSuresi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public string EkleyenKullanici { get; set; }
        public string ResimYolu {  get; set; }
    }
}
