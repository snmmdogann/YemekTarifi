using YemekTarifi.Modeller;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IKategoriService
{
    Task<List<KategoriModel>> GetirKategorilerAsync();
    
   
    

}
