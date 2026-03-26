using YemekTarifi.Modeller;
using YemekTarifi.Servisler;
using Microsoft.Maui.Storage;
using System.IO;
using System.Net.Http.Headers;

namespace YemekTarifi;

public partial class TarifGuncellemeSayfasi : ContentPage
{

    private IKategoriService _kategoriService = new KategoriService();
    private List<KategoriModel> _kategoriler;
    private Tarifler _mevcutTarif;
    private string secilenResimYolu = "";

    public TarifGuncellemeSayfasi(Tarifler tarif)
    {
        InitializeComponent();
        _mevcutTarif = tarif;

        VerileriYukle();
        KategorileriYukle();
    }

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void KategorileriYukle()
    {
        _kategoriler = await _kategoriService.GetirKategorilerAsync();


        kategoriPicker.ItemsSource = _kategoriler.Select(k => k.Ad).ToList();

        var ilkKategoriId = _mevcutTarif.TarifKategoriler.FirstOrDefault()?.KategoriId ?? 0;

        var kategoriIndex = _kategoriler.FindIndex(k => k.KategoriId == ilkKategoriId);
        if (kategoriIndex >= 0)
            kategoriPicker.SelectedIndex = kategoriIndex;
    }



    private void VerileriYukle()
    {
        adEntry.Text = _mevcutTarif.Ad;
        malzemelerEditor.Text = _mevcutTarif.Malzemeler;
        yapilisEditor.Text = _mevcutTarif.Yapilis;
        pisirmeSuresiEntry.Text = _mevcutTarif.PisirmeSuresi.ToString();

        if (!string.IsNullOrEmpty(_mevcutTarif.ResimUrl))
        {
            resimGoruntuleme.Source = ImageSource.FromUri(new Uri(_mevcutTarif.ResimUrl));
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
            await DisplayAlert("Baþarýlý", "Resim kaydedildi.", "Tamam");
        }
    }


    private async void GuncelleTarif_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(pisirmeSuresiEntry.Text) ||
            !int.TryParse(pisirmeSuresiEntry.Text, out int pisirmeSuresi) || pisirmeSuresi <= 0)
        {
            await DisplayAlert("Uyarý", "Lütfen geçerli bir piþirme süresi girin.", "Tamam");
            return;
        }
        if (kategoriPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Uyarý", "Lütfen bir kategori seçin.", "Tamam");
            return;
        }

        // Form verilerini güncelle
        _mevcutTarif.Ad = adEntry.Text?.Trim();
        _mevcutTarif.Malzemeler = malzemelerEditor.Text?.Trim();
        _mevcutTarif.Yapilis = yapilisEditor.Text?.Trim();
        _mevcutTarif.PisirmeSuresi = pisirmeSuresi;

        using var content = new MultipartFormDataContent();

        // Resim varsa ekle
        if (!string.IsNullOrEmpty(secilenResimYolu) && File.Exists(secilenResimYolu))
        {
            var extension = Path.GetExtension(secilenResimYolu).ToLowerInvariant();
            var mimeType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            var imageStream = File.OpenRead(secilenResimYolu);
            var imageContent = new StreamContent(imageStream);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            content.Add(imageContent, "resim", Path.GetFileName(secilenResimYolu));
        }

        // Diðer alanlarý ekle
        content.Add(new StringContent(_mevcutTarif.Id.ToString()), "id");
        content.Add(new StringContent(_mevcutTarif.Ad ?? ""), "ad");
        content.Add(new StringContent(_mevcutTarif.Malzemeler ?? ""), "malzemeler");
        content.Add(new StringContent(_mevcutTarif.Yapilis ?? ""), "yapilis");
        content.Add(new StringContent(_mevcutTarif.PisirmeSuresi.ToString()), "pisirmeSuresi");


        if (!string.IsNullOrWhiteSpace(_mevcutTarif.EkleyenKullanici))
            content.Add(new StringContent(_mevcutTarif.EkleyenKullanici), "ekleyenKullanici");


        if (kategoriPicker.SelectedIndex >= 0)
        {
            var secilenKategori = _kategoriler[kategoriPicker.SelectedIndex];
            content.Add(new StringContent(secilenKategori.KategoriId.ToString()), "kategoriIdler");
        }
        

        try
        {
            var client = new HttpClient();
            var response = await client.PutAsync("https://localhost:7213/tarif/guncelleform", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Baþarýlý", "Tarif güncellendi.", "Tamam");
                await Navigation.PopAsync();
            }
            else
            {
                var hata = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Hata", $"Tarif güncellenemedi:\n{hata}", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Beklenmeyen bir hata oluþtu:\n{ex.Message}", "Tamam");
        }
    }


}