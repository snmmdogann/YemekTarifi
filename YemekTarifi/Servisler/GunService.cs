using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public class GunService : IGunService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7213/gun";

        public GunService()
        {
            _httpClient = new HttpClient();
        }

        // Tüm günleri listele
        public async Task<List<GunModel>> GetirGunlerAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}/listele");
                if (!response.IsSuccessStatusCode)
                    return new List<GunModel>();

                return await response.Content.ReadFromJsonAsync<List<GunModel>>();
            }
            catch
            {
                return new List<GunModel>();
            }
        }

        /*
        // ID ile gün getir
        public async Task<GunModel?> GetirGunByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GunModel>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return null;
            }
        }

        // Yeni gün ekle
        public async Task<bool> EkleGunAsync(GunModel gun)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/ekle", gun);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Gün güncelle
        public async Task<bool> GuncelleGunAsync(int id, GunModel gun)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/{id}", gun);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Gün sil
        public async Task<bool> SilGunAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        */
    }
}
