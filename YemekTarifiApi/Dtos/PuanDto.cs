namespace YemekTarifiApi.Dtos
{
    public class PuanDto
    {
        public int PuanId { get; set; }
        public int TarifId { get; set; }
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; }
        public int Deger { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public string Yorum { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
    }
}
