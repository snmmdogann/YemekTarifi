
using YemekTarifi.Modeller;
using YemekTarifi.Servisler;

namespace YemekTarifi;

public partial class KategoriDetaySayfasi : ContentPage
{
    private IKullaniciService _kullaniciService;

    private int _kategoriId;
    private string _kategoriAd;

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Önceki sayfaya döner
    }

    private List<Tarifler> tumYemekler = new List<Tarifler>();

    public List<string> MenuItems { get; set; } = new List<string>
    {
        "Favoriler",
        "Þifre Deðiþtir",
        "Tariflerim",
        "Yorumlarým",
        "Haftalýk Yemek Programým",
        "Çýkýþ Yap"
    };

    public KategoriDetaySayfasi(int kategoriId, string kategoriAd)
    {
        InitializeComponent();

        _kategoriId = kategoriId;
        _kategoriAd = kategoriAd;
        Title = _kategoriAd;

        // IKullaniciService'yi DependencyService üzerinden alýyoruz
        _kullaniciService = DependencyService.Get<IKullaniciService>();

        MenuCollectionView.ItemsSource = MenuItems;

        _ = TarifleriYukle();

        MessagingCenter.Subscribe<object>(this, "TarifEklendi", async (sender) =>
        {
            await TarifleriYukle();
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Menü animasyonu
        MenuCollectionView.Opacity = 0;
        MenuCollectionView.TranslationX = 300;

        // Arama motoru animasyonu
        yemekAramaBar.Opacity = 0;
        yemekAramaBar.TranslationX = 300;

        // Animasyonlarý birlikte çalýþtýr 
        await Task.WhenAll(
            MenuCollectionView.FadeTo(1, 1500, Easing.CubicOut),
            MenuCollectionView.TranslateTo(0, 0, 1500, Easing.CubicOut),
          yemekAramaBar.FadeTo(1, 1500, Easing.CubicOut),
          yemekAramaBar.TranslateTo(0, 0, 1500, Easing.CubicOut)
        );
    }



    private async Task TarifleriYukle()
    {
        try
        {
            var servis = new TarifKategoriServisi();
            tumYemekler = await servis.GetirTarifleriKategoriyeGoreAsync(_kategoriId);

            yemekListView.ItemsSource = tumYemekler.Any() ? tumYemekler : null;

            if (!tumYemekler.Any())
                await DisplayAlert("Bilgi", "Bu kategoriye ait yemek bulunamadý.", "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Yemekler yüklenemedi: {ex.Message}", "Tamam");
        }

    }

    private void YemekAramaBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string aramaKelimesi = e.NewTextValue?.ToLower() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(aramaKelimesi))
        {
            yemekListView.ItemsSource = tumYemekler;
        }
        else
        {
            var filtrelenmis = tumYemekler.Where(t => t.Ad.ToLower().Contains(aramaKelimesi)).ToList();
            yemekListView.ItemsSource = filtrelenmis;
        }
    }

    private void YemekAramaBar_SearchButtonPressed(object sender, EventArgs e)
    {
        YemekAramaBar_TextChanged(sender, new TextChangedEventArgs("", yemekAramaBar.Text));
    }

    private async void TarifSecildi(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Tarifler secilenTarif)
        {
            await Navigation.PushAsync(new TariflerSayfasi(secilenTarif.Id, secilenTarif.Ad));
            yemekListView.SelectedItem = null;
        }
    }

    private async void MenuCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var secilen = e.CurrentSelection.FirstOrDefault() as string;

        if (secilen == null)
            return;


        switch (secilen)
        {
            case "Favoriler":
                await Navigation.PushAsync(new FavorilerSayfasi());
                break;

            case "Þifre Deðiþtir":
                await Navigation.PushAsync(new SifreDegistirSayfasi());
                break;

            case "Tariflerim":
                await Navigation.PushAsync(new TariflerimSayfasi());
                break;

            case "Yorumlarým":
                await Navigation.PushAsync(new YorumlarimSayfasi());
                break;

            case "Haftalýk Yemek Programým":
                await Navigation.PushAsync(new HaftalikPlanSayfasi());
                break;

            case "Çýkýþ Yap":
                if (_kullaniciService != null)
                {
                    await _kullaniciService.LogoutAsync();
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    await DisplayAlert("Hata", "Çýkýþ yapýlamadý.", "Tamam");
                }
                break;
        }
           ((CollectionView)sender).SelectedItem = null;
    }
}