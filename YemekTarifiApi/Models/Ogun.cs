namespace YemekTarifiApi.Models
{
    public class Ogun
    {
        public int Id { get; set; }
        public string Ad { get; set; }

        public ICollection<HaftalikPlan> HaftalikPlanlar { get; set; }
    }
}
