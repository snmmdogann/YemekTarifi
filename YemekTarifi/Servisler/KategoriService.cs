using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public class KategoriService : IKategoriService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7213/kategori";

        public KategoriService()
        {
            _httpClient = new HttpClient();
        }

        // Tüm kategorileri getir
        public async Task<List<KategoriModel>> GetirKategorilerAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}/listele");
                if (!response.IsSuccessStatusCode)
                    return new List<KategoriModel>();

                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<KategoriModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<KategoriModel>();
            }
            catch
            {
                return new List<KategoriModel>();
            }
        }
    }
}
