using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class TarifEkleDtoModel:TarifBaseModel
    {

        public string Resim { get; set; }
        public override string Hazirla()
        {
            return $"Yeni tarif ekleniyor: {Ad}";
        }
    }
}
