namespace YemekTarifiApi.Dtos
{
    public class PlanDto
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int GunId { get; set; }
        public int OgunId { get; set; }
        public int TarifId { get; set; }
        public string Gun { get; set; }
        public string Ogun { get; set; }
        public string TarifAdi { get; set; }
    }
}
