using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekTarifi.Modeller;
using YemekTarifi.Models;

namespace YemekTarifi.Servisler
{
    public interface IKullaniciService
    {
        Task<UserModel?> GetLoggedInUserAsync();
        Task LogoutAsync();
        Task<UserModel?> GetUserByIdAsync(string kullaniciId);
        Task<UserModel?> GirisYapAsync(KullaniciGirisModel model);
        Task<bool> UyeOlAsync(KullaniciUyeOlModel model);
    }
}
