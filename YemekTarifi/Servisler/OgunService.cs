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
    public class OgunService:IOgunService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7213/ogun";

        public OgunService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<OgunModel>> GetirOgunlerAsync()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/listele");
            if (!response.IsSuccessStatusCode)
                return new List<OgunModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OgunModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OgunModel>();
        }

        /*public async Task<OgunModel?> GetirOgunByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OgunModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<bool> EkleOgunAsync(OgunModel ogun)
        {
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/ekle", ogun);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> GuncelleOgunAsync(int id, OgunModel ogun)
        {
            var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/{id}", ogun);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SilOgunAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }*/
    }
}

