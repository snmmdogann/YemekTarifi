
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class HaftalikPlanModel
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }

        public int GunId { get; set; }
        public string Gun { get; set; } 
        public string GunAd {  get; set; }
        public int OgunId { get; set; }
        public string Ogun { get; set; }
        public string OgunAd {  get; set; }

        public int TarifId { get; set; }
        public string TarifAdi { get; set; }
    }
}
