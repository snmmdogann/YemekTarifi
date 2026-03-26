using System.Text.Json;
using YemekTarifi.Modeller;
using YemekTarifi.Servisler;

namespace YemekTarifi;
public partial class YorumlarimSayfasi : ContentPage
{
    private IPuanService _puanService;
    private IKullaniciService _kullaniciService;

    private int _girisYapanKullaniciId;

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Önceki sayfaya döner
    }


    public YorumlarimSayfasi()
    {
        InitializeComponent();
        _puanService = new PuanService();
        _kullaniciService = new KullaniciServisi();


    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await KullaniciBilgisiniAlVePuanlariYukle();

        await Task.Delay(300);


        var yorumKartlari = YorumlarCollectionView
            .LogicalChildren
            .OfType<Frame>()
            .ToArray();
        //YorumlarCollectionView içinde bulunan tüm Frame tipindeki görsel öðeleri(kartlarý) 
        //tek tek yakalayýp bir diziye almaktýr.

        foreach (var kart in yorumKartlari)
        {
            kart.Opacity = 0;
            kart.TranslationX = 300;
        }


        foreach (var kart in yorumKartlari)
        {
            await Task.WhenAll(
                kart.FadeTo(1, 800, Easing.CubicOut),
                kart.TranslateTo(0, 0, 800, Easing.CubicOut)
            );
        }
    }


    private async Task KullaniciBilgisiniAlVePuanlariYukle()
    {
        try
        {
            var kullanici = await _kullaniciService.GetLoggedInUserAsync();

            if (kullanici == null)
            {
                await DisplayAlert("Hata", "Giriþ yapmýþ kullanýcý bulunamadý.", "Tamam");
                return;
            }

            _girisYapanKullaniciId = kullanici.KullaniciId;

            var kullaniciPuanlari = await _puanService.KullaniciTarifPuanlariniGetirAsync(_girisYapanKullaniciId);

            if (kullaniciPuanlari == null || !kullaniciPuanlari.Any())
            {
                await DisplayAlert("Bilgi", "Henüz yorum yapýlmamýþ.", "Tamam");
                YorumlarCollectionView.ItemsSource = null;
                return;
            }

            YorumlarCollectionView.ItemsSource = kullaniciPuanlari;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Yorumlar yüklenemedi: {ex.Message}", "Tamam");
        }
    }

    private async void SilPuan_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var secilenPuan = button?.CommandParameter as PuanModel;

        if (secilenPuan == null)
            return;

        bool onay = await DisplayAlert("Sil", $"{secilenPuan.TarifAdi} tarifine ait yorumunuzu silmek istiyor musunuz?", "Evet", "Hayýr");

        if (onay)
        {
            try
            {
                bool sonuc = await _puanService.SilPuanAsync(_girisYapanKullaniciId, secilenPuan.TarifId);

                if (sonuc)
                {
                    var liste = YorumlarCollectionView.ItemsSource as List<PuanModel>;
                    liste.Remove(secilenPuan);
                    YorumlarCollectionView.ItemsSource = null;
                    YorumlarCollectionView.ItemsSource = liste;

                    await DisplayAlert("Baþarýlý", "Yorumunuz silindi.", "Tamam");
                }
                else
                {
                    await DisplayAlert("Hata", "Yorum silinemedi.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Hata oluþtu: {ex.Message}", "Tamam");
            }
        }
    }

    private async void GuncellePuan_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var secilenPuan = button?.CommandParameter as PuanModel;

        if (secilenPuan != null)
        {
            await Navigation.PushAsync(new PuanGuncellemeSayfasi(secilenPuan));
        }
    }


}