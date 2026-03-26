using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public abstract class TarifBaseModel
    {
        public string Ad { get; set; }
        public string Malzemeler { get; set; }
        public string Yapilis { get; set; }
        public int PisirmeSuresi { get; set; }
        public string EkleyenKullanici { get; set; }
        public abstract string Hazirla();
    }
}
