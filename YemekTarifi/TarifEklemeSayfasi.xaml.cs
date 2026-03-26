using System.Net.Http.Headers;
using System.Text.Json;
using YemekTarifi.Modeller;
using YemekTarifi.Servisler;

namespace YemekTarifi;

public partial class TarifEklemeSayfasi : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();

    private KategoriService _kategoriService = new KategoriService();
    private ITarifKategoriServisi _tarifKategoriServisi;


    private string secilenResimYolu = "";

    public TarifEklemeSayfasi()
    {
        InitializeComponent();

        _tarifKategoriServisi = new TarifKategoriServisi();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var kategoriler = await _kategoriService.GetirKategorilerAsync();
        if (kategoriler != null && kategoriler.Any())
        {
            kategoriPicker.ItemsSource = kategoriler;
            kategoriPicker.ItemDisplayBinding = new Binding("Ad");
        }
        else
        {
            await DisplayAlert("Uyarı", "Kategori bulunamadı.", "Tamam");
        }
    }

    private async void ResimSecButon_Clicked(object sender, EventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Bir resim seç",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            var stream = await result.OpenReadAsync();
            var filePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);

            using (var fileStream = File.OpenWrite(filePath))
            {
                await stream.CopyToAsync(fileStream);
            }

            secilenResimYolu = filePath;
            resimGoruntuleme.Source = ImageSource.FromFile(secilenResimYolu);
            await DisplayAlert("Başarılı", "Resim seçildi.", "Tamam");
        }
    }

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void EkleButon_Clicked(object sender, EventArgs e)
    {
        var secilenKategori = (KategoriModel)kategoriPicker.SelectedItem;
        if (secilenKategori == null)
        {
            await DisplayAlert("Uyarı", "Lütfen bir kategori seçin.", "Tamam");
            return;
        }

        if (string.IsNullOrEmpty(secilenResimYolu) || !File.Exists(secilenResimYolu))
        {
            await DisplayAlert("Uyarı", "Lütfen bir resim seçin.", "Tamam");
            return;
        }

        if (string.IsNullOrWhiteSpace(pisirmeSuresiEntry.Text) || !int.TryParse(pisirmeSuresiEntry.Text, out int pisirmeSuresi) || pisirmeSuresi <= 0)
        {
            await DisplayAlert("Uyarı", "Lütfen geçerli bir pişirme süresi girin.", "Tamam");
            return;
        }


        var multipartContent = new MultipartFormDataContent();

        var stream = File.OpenRead(secilenResimYolu);
        var fileContent = new StreamContent(stream);

        var ext = Path.GetExtension(secilenResimYolu).ToLower();
        string mimeType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

        multipartContent.Add(fileContent, "Resim", Path.GetFileName(secilenResimYolu));

        multipartContent.Add(new StringContent(adEntry.Text ?? ""), "Ad");
        multipartContent.Add(new StringContent(malzemeEntry.Text ?? ""), "Malzemeler");
        multipartContent.Add(new StringContent(yapilisEditor.Text ?? ""), "Yapilis");
        multipartContent.Add(new StringContent(Preferences.Get("kullaniciAdi", "varsayilan")), "EkleyenKullanici");
        multipartContent.Add(new StringContent(pisirmeSuresi.ToString()), "PisirmeSuresi");

        try
        {
            var apiUrl = "https://localhost:7213/tarif/ekle";

            var response = await _httpClient.PostAsync(apiUrl, multipartContent);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var yeniTarif = JsonSerializer.Deserialize<Tarifler>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                var dto = new TarifEkleDtoModel
                {
                    Ad = yeniTarif.Ad,
                    Malzemeler = yeniTarif.Malzemeler,
                    Yapilis = yeniTarif.Yapilis,
                    PisirmeSuresi = yeniTarif.PisirmeSuresi,
                    EkleyenKullanici = yeniTarif.EkleyenKullanici
                };

                string mesaj = dto.Hazirla();
                await DisplayAlert("Bilgi", mesaj, "Tamam");
                if (yeniTarif != null && yeniTarif.Id > 0)
                {

                    bool iliskiEklendi = await _tarifKategoriServisi.TarifKategorisiEkleAsync(yeniTarif.Id, secilenKategori.KategoriId);

                    if (iliskiEklendi)
                    {
                        await DisplayAlert("Başarılı", "Tarif ve kategori ilişkisi eklendi.", "Tamam");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Uyarı", "Tarif eklendi ama kategori ilişkisi kurulamadı.", "Tamam");
                    }
                }
                else
                {
                    await DisplayAlert("Hata", "Tarif eklenemedi. Sunucu hatası.", "Tamam");
                }
            }
            else
            {
                await DisplayAlert("Hata", $"Sunucu hatası: {response.StatusCode}", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"İstek sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }
}
