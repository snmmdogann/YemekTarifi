using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YemekTarifi.Modeller;
using YemekTarifi.Servisler;

namespace YemekTarifi.Servisler
{
    public class FavoriService : IFavoriService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7213/favori";

        public FavoriService()
        {
            _httpClient = new HttpClient();
        }


        // Kullanıcıya ait favorileri getirir
        public async Task<List<FavoriModel>> GetirKullaniciFavorileriAsync(int kullaniciId)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/kullanici/{kullaniciId}");
            if (!response.IsSuccessStatusCode)
                return new List<FavoriModel>();

            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<FavoriModel>();
            }
            return JsonSerializer.Deserialize<List<FavoriModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // Yeni favori ekler
        public async Task<bool> EkleFavoriAsync(FavoriModel favori)
        {
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/ekle", favori);
            return response.IsSuccessStatusCode;
        }

        // Favori siler
        public async Task<bool> SilFavoriAsync(int kullaniciId, int tarifId)
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/sil/{kullaniciId}/{tarifId}");
            return response.IsSuccessStatusCode;
        }
    }
}

