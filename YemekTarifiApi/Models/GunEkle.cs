namespace YemekTarifiApi.Models
{
    public class GunEkle
    {
        public string Ad { get; set; }

        public ICollection<HaftalikPlan> HaftalikPlanlar { get; set; }
    }
}
