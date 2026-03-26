using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public interface ITarifKategoriServisi
    {
        Task<List<Tarifler>> GetirTarifleriKategoriyeGoreAsync(int kategoriId);

        Task<bool> TarifKategorisiEkleAsync(int tarifId, int kategoriId);
    }
}
