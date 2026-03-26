using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public interface IFavoriService
    {
        Task<bool> EkleFavoriAsync(FavoriModel favori); 
        Task<List<FavoriModel>> GetirKullaniciFavorileriAsync(int kullaniciId); 
        Task<bool> SilFavoriAsync(int kullaniciId, int tarifId);
    }
}
