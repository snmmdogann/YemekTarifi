namespace YemekTarifiApi.Models
{
    public class HaftalikPlan
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int GunId { get; set; }
        public int OgunId { get; set; }
        public int TarifId { get; set; }
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        // Navigation properties
        public Kullanici Kullanici { get; set; }
        public Gun Gun { get; set; }
        public Ogun Ogun { get; set; }
        public Tarif Tarif { get; set; }
    }
}
