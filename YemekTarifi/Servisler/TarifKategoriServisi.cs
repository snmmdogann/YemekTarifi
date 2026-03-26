using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public class TarifKategoriServisi:ITarifKategoriServisi
    {
        private readonly HttpClient _httpClient;

        public TarifKategoriServisi()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Tarifler>> GetirTarifleriKategoriyeGoreAsync(int kategoriId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7213/tarifkategori/listele?kategoriId={kategoriId}");

            if (!response.IsSuccessStatusCode)
                return new List<Tarifler>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Tarifler>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Tarifler>();
        }

        public async Task<bool> TarifKategorisiEkleAsync(int tarifId, int kategoriId)
        {
            var model = new { TarifId = tarifId, KategoriId = kategoriId };
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7213/tarifkategori/ekle", model);
            return response.IsSuccessStatusCode;
        }



    }
}
