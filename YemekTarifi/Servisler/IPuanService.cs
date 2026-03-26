using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Modeller;
namespace YemekTarifi.Servisler
{
    public interface IPuanService
    {
        Task<bool> GuncellePuanAsync(PuanModel puan);
        Task<List<PuanModel>> TarifPuanlariniGetirAsync(int tarifId);
        Task<List<PuanModel>> KullaniciTarifPuanlariniGetirAsync(int kullaniciId);
        Task<bool> SilPuanAsync(int kullaniciId, int TarifId);
        Task<bool> PuanGonderAsync(PuanModel puan);

    }
}
