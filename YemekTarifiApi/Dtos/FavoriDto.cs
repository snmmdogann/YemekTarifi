namespace YemekTarifiApi.Dtos
{
    public class FavoriDto
    {
        public int KullaniciId { get; set; }
        public string TarifAdi { get; set; }
        public int TarifId { get; set; } 
        public DateTime EklenmeTarihi { get; set; }
    }
}