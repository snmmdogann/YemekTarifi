using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public interface IOgunService
    {
        Task<List<OgunModel>> GetirOgunlerAsync();
        /*Task<OgunModel?> GetirOgunByIdAsync(int id);
        Task<bool> EkleOgunAsync(OgunModel ogun);
        Task<bool> GuncelleOgunAsync(int id, OgunModel ogun);
        Task<bool> SilOgunAsync(int id);*/
    }
}
