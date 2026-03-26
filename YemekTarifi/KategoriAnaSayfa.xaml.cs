using System.Text.Json;
using YemekTarifi.Modeller;

using YemekTarifi.Servisler;

namespace YemekTarifi;

public partial class KategoriAnaSayfa : ContentPage
{
    private IKullaniciService _userService;
    private ITarifService _tarifService;
    private IKategoriService _kategoriService;
    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Önceki sayfaya döner
    }

    // Menü öğeleri 
    private readonly List<string> menuItems = new()
    {
        "Favoriler",
        "Şifre Değiştir",
        "Tariflerim",
        "Yorumlarım",
        "Haftalık Yemek Programım",
        "Çıkış Yap"
    };
    private List<Tarifler> tumTarifler;
    public KategoriAnaSayfa()
    {
        InitializeComponent();

        _userService = new KullaniciServisi();
        _kategoriService = new KategoriService();
        _tarifService = new TarifService();

        KategorileriYukle();

        MenuCollectionView.ItemsSource = menuItems;
        MenuCollectionView.IsVisible = false;

        TarifleriYukle();
    }

    private async Task KategorileriYukle()
    {
        try
        {
            var kategoriler = await _kategoriService.GetirKategorilerAsync();

            if (kategoriler == null || kategoriler.Count == 0)
            {
                await DisplayAlert("Hata", "Kategoriler alınamadı", "Tamam");
                return;
            }

            kategoriListView.ItemsSource = kategoriler;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Kategori yüklenemedi: {ex.Message}", "Tamam");
        }
    }



    private Tarifler gununYemegi;
    private async Task TarifleriYukle()
    {
        try
        {
            var servis = new TarifService(); 
            tumTarifler = await servis.GetirTariflerAsync();

            if (tumTarifler != null && tumTarifler.Count > 0)
            {
                int index = DateTime.Today.DayOfYear % tumTarifler.Count; 
                gununYemegi = tumTarifler[index];

                GununYemegiLabel.Text = gununYemegi.Ad;
                gununYemegiImage.Source = _tarifService.ResimUrlTamamla(gununYemegi.ResimYolu);

                string tamamlanmisUrl = _tarifService.ResimUrlTamamla(gununYemegi.ResimYolu);


                gununYemegiImage.Source = tamamlanmisUrl;


                GununYemegiLayout.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Hata", "Hiç tarif bulunamadı", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Tarif yüklenemedi: {ex.Message}", "Tamam");
        }
    }

    private async void GununYemegi_Tapped(object sender, EventArgs e)
    {
        if (gununYemegi != null)
        {
            await Navigation.PushAsync(new TariflerSayfasi(gununYemegi.Id, gununYemegi.Ad));
        }
    }


    private async void KategoriSecildi(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is KategoriModel secilenKategori)
        {
            await Navigation.PushAsync(new KategoriDetaySayfasi(secilenKategori.KategoriId, secilenKategori.Ad));
            kategoriListView.SelectedItem = null;
        }
    }

    // Basit benzerlik oranı hesaplayan yardımcı fonksiyon
    private double BenzerlikHesapla(string kaynak, string hedef)
    {
        kaynak = kaynak.ToLower();
        hedef = hedef.ToLower();

        int ortakKarakter = kaynak.Intersect(hedef).Count();
        return (double)ortakKarakter / Math.Max(kaynak.Length, hedef.Length);
    }
    //ARAMA MOTORU

    private async void YemekAramaBar_SearchButtonPressed(object sender, EventArgs e)
    {
        string aramaKelimesi = yemekAramaBar.Text?.Trim().ToLower();

        if (string.IsNullOrEmpty(aramaKelimesi) || tumTarifler == null)
            return;

        var enBenzerTarif = tumTarifler
            .Select(t => new
            {
                Tarif = t,
                Benzerlik = BenzerlikHesapla(t.Ad ?? "", aramaKelimesi)
            })
            .OrderByDescending(x => x.Benzerlik)
            .FirstOrDefault();

        // Belirli bir eşik değer üzerinde ise kabul et
        if (enBenzerTarif != null && enBenzerTarif.Benzerlik > 0.5)
        {
            await Navigation.PushAsync(new TariflerSayfasi(enBenzerTarif.Tarif.Id, enBenzerTarif.Tarif.Ad));
            yemekAramaBar.Text = string.Empty;
        }
        else
        {
            await DisplayAlert("Bilgi", "Tarif bilgisi bulunamadı.", "Tamam");
        }
    }

    // Menü butonuna tıklayınca aç/kapa
    private void MenuButton_Clicked(object sender, EventArgs e)
    {
        MenuCollectionView.IsVisible = !MenuCollectionView.IsVisible;
    }


    // Menüden seçim yapınca işlemleri çalıştır
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

            case "Şifre Değiştir":
                await Navigation.PushAsync(new SifreDegistirSayfasi());
                break;

            case "Tariflerim":
                await Navigation.PushAsync(new TariflerimSayfasi());
                break;

            case "Yorumlarım":
                await Navigation.PushAsync(new YorumlarimSayfasi());
                break;

            case "Haftalık Yemek Programım":


                await Navigation.PushAsync(new HaftalikPlanSayfasi());

                break;


            case "Çıkış Yap":
                if (_userService != null)
                {
                    await _userService.LogoutAsync();   
                    await Navigation.PopToRootAsync();  
                }
                else
                {
                    await DisplayAlert("Hata", "Çıkış yapılamadı.", "Tamam");
                }
                break;
        }



            ((CollectionView)sender).SelectedItem = null;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        GununYemegiLayout.TranslationX = 500;
        GununYemegiLayout.Opacity = 0;

        yemekAramaBar.TranslationX = 500;
        yemekAramaBar.Opacity = 0;

        kategoriListView.TranslationX = 500;
        kategoriListView.Opacity = 0;

        await Task.WhenAll(
            GununYemegiLayout.TranslateTo(0, 0, 700, Easing.CubicOut),
            GununYemegiLayout.FadeTo(1, 700)
        );

        await Task.WhenAll(
            yemekAramaBar.TranslateTo(0, 0, 700, Easing.CubicOut),
            yemekAramaBar.FadeTo(1, 700)
        );

        await Task.WhenAll(
            kategoriListView.TranslateTo(0, 0, 700, Easing.CubicOut),
            kategoriListView.FadeTo(1, 700)
        );
    }


}