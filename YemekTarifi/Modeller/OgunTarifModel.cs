using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class OgunTarifModel
    {
        public string Ad { get; set; } 

        public ObservableCollection<TarifGorunum> Tarifler { get; set; } = new ObservableCollection<TarifGorunum>();
    }
}
