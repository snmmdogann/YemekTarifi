using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

public interface ITarifService
{
    Task<bool> SilTarifAsync(int id);
    Task<List<Tarifler>> GetirTariflerAsync();
    Task<Tarifler?> GetirTarifByIdAsync(int tarifId);
    string ResimUrlTamamla(string? resimUrl);
    Task<bool> GuncelleTarifFormAsync(Tarifler model);
   

}

