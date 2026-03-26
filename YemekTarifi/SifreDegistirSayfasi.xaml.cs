using System.Net.Http;
using System.Text;
using System.Text.Json;
using YemekTarifi.Servisler;

namespace YemekTarifi;

public partial class SifreDegistirSayfasi : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly KullaniciServisi _kullaniciServisi;
    public SifreDegistirSayfasi()
    {
        InitializeComponent();
        _kullaniciServisi = new KullaniciServisi();

    }


    private async void SifreDegistirButton_Clicked(object sender, EventArgs e)
    {
        var aktifKullanici = await _kullaniciServisi.GetLoggedInUserAsync();
        if (aktifKullanici == null || string.IsNullOrEmpty(aktifKullanici.KullaniciAdi))
        {
            await DisplayAlert("Hata", "Kullanýcý bilgisi alýnamadý.", "Tamam");
            return;
        }

        string kullaniciAdi = aktifKullanici.KullaniciAdi;
        // buradan sonra kullan

        string eskiSifre = EskiSifreEntry.Text;
        string yeniSifre = YeniSifreEntry.Text;
        string yeniSifreTekrar = YeniSifreTekrarEntry.Text;



        if (string.IsNullOrWhiteSpace(eskiSifre) || string.IsNullOrWhiteSpace(yeniSifre) || string.IsNullOrWhiteSpace(yeniSifreTekrar))
        {
            await DisplayAlert("Uyarý", "Lütfen tüm alanlarý doldurun.", "Tamam");
            return;
        }

        if (yeniSifre != yeniSifreTekrar)
        {
            await DisplayAlert("Uyarý", "Yeni þifre ve tekrar þifre eþleþmiyor.", "Tamam");
            return;
        }

        try
        {
            var jsonData = new
            {
                kullaniciAdi = kullaniciAdi,
                eskiSifre = eskiSifre,
                yeniSifre = yeniSifre,
                yeniSifreTekrar = yeniSifreTekrar,
            };

            string jsonString = JsonSerializer.Serialize(jsonData);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            string url = "https://localhost:7213/sifredegistir"; 

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Baþarýlý", "Þifreniz baþarýyla deðiþtirildi.", "Tamam");
                await Navigation.PopAsync();
            }
            else
            {
                string hataMesaji = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Hata", $"Þifre deðiþtirme iþlemi baþarýsýz: {hataMesaji}", "Tamam");

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Bir hata oluþtu: " + ex.Message, "Tamam");
        }
    }

}