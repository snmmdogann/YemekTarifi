using YemekTarifi.Servisler;
using YemekTarifi.Modeller;
namespace YemekTarifi;

public partial class FavorilerSayfasi : ContentPage
{
    private IFavoriService _favoriService;
    private IKullaniciService _kullaniciService;

    private int _girisYapanKullaniciId;

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Önceki sayfaya döner
    }

    

    public FavorilerSayfasi()
    {
        InitializeComponent();//xaml kýsmýný baþlatýr.
        _favoriService = new FavoriService();
        _kullaniciService = new KullaniciServisi();

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_kullaniciService == null)
        {
            await DisplayAlert("Hata", "Kullanýcý servisi null!", "Tamam");
            return;
        }

        var kullanici = await _kullaniciService.GetLoggedInUserAsync();
        if (kullanici == null)
        {
            await DisplayAlert("Hata", "Kullanýcý bilgisi yok!", "Tamam");
            return;
        }

        await KullaniciBilgisiniAlVeFavorileriYukle();

        // Animasyon için baþlangýç ayarlarý
        FavorilerListView.TranslationX = 500;
        FavorilerListView.Opacity = 0;

        // Kaydýrmalý ve opaklýk deðiþimi animasyonu
        await Task.WhenAll(
            FavorilerListView.TranslateTo(0, 0, 700, Easing.CubicOut),
            FavorilerListView.FadeTo(1, 700)
        );
    }


    private async Task KullaniciBilgisiniAlVeFavorileriYukle()
    {
        var kullanici = await _kullaniciService.GetLoggedInUserAsync();

        if (kullanici != null)
        {
            _girisYapanKullaniciId = kullanici.KullaniciId;

            var kullaniciFavorileri = await _favoriService.GetirKullaniciFavorileriAsync(_girisYapanKullaniciId);

            foreach (var favori in kullaniciFavorileri)
            {
                System.Diagnostics.Debug.WriteLine($"TarifId: {favori.TarifId}, TarifAdi: {favori.TarifAdi}");
            }

            FavorilerListView.ItemsSource = kullaniciFavorileri;
        }
        else
        {
            await DisplayAlert("Hata", "Kullanýcý bilgisi bulunamadý!", "Tamam");
        }
    }


    private async void SilFavori_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter is FavoriModel favori)
        {
            bool sonuc = await _favoriService.SilFavoriAsync(favori.KullaniciId, favori.TarifId);
            if (sonuc)
            {

                await DisplayAlert("Baþarýlý", "Favori kaldýrýldý.", "Tamam");


                await KullaniciBilgisiniAlVeFavorileriYukle();  // Listeyi yenile
            }
            else
            {
                await DisplayAlert("Kontrol", $"KullaniciId: {favori.KullaniciId}\nTarifId: {favori.TarifId}", "Tamam");
                await DisplayAlert("Hata", "Favori kaldýrýlamadý.", "Tamam");
            }
        }
    }

    private async void FavorilerListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var secilenFavori = e.SelectedItem as FavoriModel;
        if (secilenFavori != null)
        {
            int Id = secilenFavori.TarifId;
            string Ad = secilenFavori.TarifAdi;


            await Navigation.PushAsync(new TariflerSayfasi(Id, Ad));
        }

    }

}