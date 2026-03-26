using System.Net.Http.Headers;
using System.Text.Json;
using YemekTarifi.Modeller;

namespace YemekTarifi.Servisler
{
    public class TarifService : ITarifService
    {
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:7213/tarif";

        public TarifService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Tarifler>> GetirTariflerAsync()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/listele");
            if (!response.IsSuccessStatusCode)
                return new List<Tarifler>();

            var json = await response.Content.ReadAsStringAsync();

            var tarifler = JsonSerializer.Deserialize<List<Tarifler>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Tarifler>();

            foreach (var tarif in tarifler)
            {
                tarif.ResimUrl = ResimUrlTamamla(tarif.ResimYolu);
            }

            return tarifler;
        }

        public async Task<Tarifler?> GetirTarifByIdAsync(int tarifId)
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/{tarifId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            var tarif = JsonSerializer.Deserialize<Tarifler>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (tarif != null)
                tarif.ResimUrl = ResimUrlTamamla(tarif.ResimYolu);

            return tarif;
        }

      
        public async Task<bool> GuncelleTarifFormAsync(Tarifler model)
        {
            using var content = new MultipartFormDataContent();

            if (!string.IsNullOrWhiteSpace(model.ResimYolu) && File.Exists(model.ResimYolu))
            {
                var extension = Path.GetExtension(model.ResimYolu).ToLowerInvariant();
                string mimeType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    _ => "application/octet-stream"
                };

                var imageContent = new StreamContent(File.OpenRead(model.ResimYolu));
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                content.Add(imageContent, "Resim", Path.GetFileName(model.ResimYolu));
            }

            content.Add(new StringContent(model.Id.ToString()), "id");
            content.Add(new StringContent(model.Ad ?? ""), "ad");
            content.Add(new StringContent(model.Malzemeler ?? ""), "malzemeler");
            content.Add(new StringContent(model.Yapilis ?? ""), "yapilis");
            content.Add(new StringContent(model.PisirmeSuresi.ToString()), "pisirmeSuresi");

            var response = await _httpClient.PutAsync($"{baseUrl}/guncelleform", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SilTarifAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public string ResimUrlTamamla(string? resimUrl)
        {
            if (string.IsNullOrEmpty(resimUrl))
                return string.Empty;

            if (resimUrl.StartsWith("http"))
                return resimUrl;

            return $"https://localhost:7213{(resimUrl.StartsWith("/") ? "" : "/")}{resimUrl}";
        }
    }
}
