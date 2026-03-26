namespace YemekTarifiApi.Models
{
    public class OgunEkle
    {
        public string Ad { get; set; }

        public ICollection<HaftalikPlan> HaftalikPlanlar { get; set; }
    }
}
