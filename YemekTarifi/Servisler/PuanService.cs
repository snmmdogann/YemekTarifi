using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public class PuanService : IPuanService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7213/puan";

        public PuanService()
        {
            _httpClient = new HttpClient();
        }

        // Belirli tarifin puanlarını getir
        public async Task<List<PuanModel>> TarifPuanlariniGetirAsync(int tarifId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}/ara?tarifId={tarifId}");

                if (!response.IsSuccessStatusCode)
                    return new List<PuanModel>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PuanModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<PuanModel>();
            }
            catch
            {
                return new List<PuanModel>();
            }
        }

        // Kullanıcının puanladığı tarifleri getir
        public async Task<List<PuanModel>> KullaniciTarifPuanlariniGetirAsync(int kullaniciId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}/kullanici/{kullaniciId}");

                if (!response.IsSuccessStatusCode)
                    return new List<PuanModel>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PuanModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<PuanModel>();
            }
            catch
            {
                return new List<PuanModel>();
            }
        }

        // Yeni puan gönder
        public async Task<bool> PuanGonderAsync(PuanModel puan)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/ekle?tarifId={puan.TarifId}", puan);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Puan güncelle
        public async Task<bool> GuncellePuanAsync(PuanModel puan)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/guncelle/{puan.PuanId}", puan);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Puan sil
        public async Task<bool> SilPuanAsync(int kullaniciId, int tarifId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{baseUrl}/sil?kullaniciId={kullaniciId}&tarifId={tarifId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
