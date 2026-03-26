using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public interface IHaftalikPlanService
    {
        Task<List<HaftalikPlanModel>> KullaniciHaftalikPlanlariniGetirAsync(int kullaniciId);
        Task<HaftalikPlanModel?> PlanEkleAsync(HaftalikPlanModel plan);
        Task<bool> PlanSilAsync(int planId);
    }
}
