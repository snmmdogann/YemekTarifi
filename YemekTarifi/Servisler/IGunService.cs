
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public interface IGunService
    {
        Task<List<GunModel>> GetirGunlerAsync();
        /*Task<GunModel?> GetirGunByIdAsync(int id);*/

        /*Task<bool> GuncelleGunAsync(int id, GunModel gun);
        Task<bool> SilGunAsync(int id);

        Task<bool> EkleGunAsync(GunModel gun);*/
    }
}
