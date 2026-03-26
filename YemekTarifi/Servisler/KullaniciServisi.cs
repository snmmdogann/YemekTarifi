using System.Net.Http.Json;
using System.Text.Json;
using YemekTarifi.Modeller;
using YemekTarifi.Models;
using Microsoft.Maui.Storage;

namespace YemekTarifi.Servisler
{
    public class KullaniciServisi : IKullaniciService
    {
        private  HttpClient _httpClient;
        private  string baseUrl = "https://localhost:7213/kullanici";
        private const string KULLANICI_KEY  = "GirisYapanKullanici";

        public KullaniciServisi()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler);
        }

        // Giriş yap
        public async Task<UserModel?> GirisYapAsync(KullaniciGirisModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/giris", model);

            if (response.IsSuccessStatusCode)
            {
                var kullanici = await response.Content.ReadFromJsonAsync<UserModel>();

                if (kullanici != null)
                {
                    KaydetGirisYapanKullanici(kullanici);
                }

                return kullanici;
            }

            return null;
        }

        // Üye ol
        public async Task<bool> UyeOlAsync(KullaniciUyeOlModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/ekle", model);
            return response.IsSuccessStatusCode;
        }

        // Giriş yapan kullanıcıyı kaydet
        public void KaydetGirisYapanKullanici(UserModel kullanici)
        {
            var json = JsonSerializer.Serialize(kullanici);
            Preferences.Default.Set(KULLANICI_KEY , json);
        }

        // Giriş yapan kullanıcıyı getir
        public async Task<UserModel?> GetLoggedInUserAsync()
        {
            if (Preferences.ContainsKey(KULLANICI_KEY ))
            {
                var json = Preferences.Default.Get(KULLANICI_KEY , "");
                return JsonSerializer.Deserialize<UserModel>(json);
            }
            return null;
        }

        // Çıkış yap
        public async Task LogoutAsync()
        {
            Preferences.Default.Remove(KULLANICI_KEY );
            await Task.CompletedTask;
        }

        // Kullanıcıyı ID'ye göre getir
        public async Task<UserModel?> GetUserByIdAsync(string kullaniciId)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/{kullaniciId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserModel>();
            }
            return null;
        }
    }
}
