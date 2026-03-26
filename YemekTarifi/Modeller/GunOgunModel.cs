using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekTarifi.Modeller
{
    public class GunOgunModel
    {
        public string Ad { get; set; } 

        public ObservableCollection<OgunTarifModel> Ogunler { get; set; } = new ObservableCollection<OgunTarifModel>();
    }
}
