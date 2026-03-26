using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public class HaftalikPlanServisi : IHaftalikPlanService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7213/haftalikplan";

        public HaftalikPlanServisi()
        {
            _httpClient = new HttpClient();
        }

        // Kullanıcının tüm haftalık planlarını getir
        public async Task<List<HaftalikPlanModel>> KullaniciHaftalikPlanlariniGetirAsync(int kullaniciId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}/kullanici/{kullaniciId}");
                if (!response.IsSuccessStatusCode)
                    return new List<HaftalikPlanModel>();

                return await response.Content.ReadFromJsonAsync<List<HaftalikPlanModel>>();
            }
            catch
            {
                return new List<HaftalikPlanModel>();
            }
        }

        // Yeni plan ekle
        public async Task<HaftalikPlanModel?> PlanEkleAsync(HaftalikPlanModel plan)
        {
            try
            {
                var dto = new HaftalikPlanModel
                {
                    KullaniciId = plan.KullaniciId,
                    GunId = plan.GunId,
                    OgunId = plan.OgunId,
                    TarifId = plan.TarifId
                };

                var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/ekle", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API Hatası: {errorMessage}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<HaftalikPlanModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hata: {ex.Message}");
                return null;
            }
        }

        // Plan sil
        public async Task<bool> PlanSilAsync(int planId)
        {
            try
            {
                Debug.WriteLine($"Plan silme isteği gönderiliyor: Id={planId}");
                var response = await _httpClient.DeleteAsync($"{baseUrl}/{planId}");
                Debug.WriteLine($"Silme isteği sonucu: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
