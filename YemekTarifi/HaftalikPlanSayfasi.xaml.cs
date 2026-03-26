using System.Collections.ObjectModel;
using YemekTarifi.Modeller;
using YemekTarifi.Servisler;

namespace YemekTarifi;

public partial class HaftalikPlanSayfasi : ContentPage
{
    private IKullaniciService _kullaniciService;
    private IHaftalikPlanService _haftalikPlanService;
    private ITarifService _tarifService;
    private IGunService _gunService;
    private IOgunService _ogunService;

    private int _girisYapanKullaniciId;

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Önceki sayfaya döner
    }

    //koleksiyonlar
    public ObservableCollection<GunModel> Gunler { get; set; } = new();
    public ObservableCollection<OgunModel> Ogunler { get; set; } = new();
    public ObservableCollection<Tarifler> Tarifler { get; set; } = new();
    public ObservableCollection<HaftalikPlanModel> HaftalikPlanlar { get; set; } = new();

    private Dictionary<(string Gun, string Ogun), List<string>> PlanlarTablo = new();

    public HaftalikPlanSayfasi()
    {
        InitializeComponent();
        BindingContext = this;

        _kullaniciService = new KullaniciServisi();
        _haftalikPlanService = new HaftalikPlanServisi();
        _tarifService = new TarifService();
        _gunService = new GunService();
        _ogunService = new OgunService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await KullaniciBilgisiniAlVeVerileriYukle();
        await GunleriYukleAsync();
        await OgunleriYukleAsync();

        PlanTabloGrid.Children.Clear();
        PlanTabloGrid.ColumnDefinitions.Clear();
        PlanTabloGrid.RowDefinitions.Clear();

        PlanlarTablo.Clear();


        // Haftalýk planlarý tabloya yükle
        foreach (var plan in HaftalikPlanlar)
        {
            var key = (plan.Gun, plan.Ogun);
            if (!PlanlarTablo.ContainsKey(key))
                PlanlarTablo[key] = new List<string>();

            PlanlarTablo[key].Add(plan.TarifAdi);

        }

        TabloOlustur();
    }

    private async Task GunleriYukleAsync()
    {
        Gunler.Clear();
        var gunler = await _gunService.GetirGunlerAsync();

        foreach (var gun in gunler)
            Gunler.Add(gun);
    }

    private async Task OgunleriYukleAsync()
    {
        Ogunler.Clear();
        var ogunler = await _ogunService.GetirOgunlerAsync();

        foreach (var ogun in ogunler)
            Ogunler.Add(ogun);
    }

    private async Task KullaniciBilgisiniAlVeVerileriYukle()
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

            var tarifler = await _tarifService.GetirTariflerAsync();
            Tarifler.Clear();
            foreach (var t in tarifler)
                Tarifler.Add(t);

            var planlar = await _haftalikPlanService.KullaniciHaftalikPlanlariniGetirAsync(_girisYapanKullaniciId);
            HaftalikPlanlar.Clear();
            foreach (var p in planlar)
                HaftalikPlanlar.Add(p);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Veriler yüklenemedi: {ex.Message}", "Tamam");
        }
    }

    private async void TarifEkle_Clicked(object sender, EventArgs e)
    {
        var secilenGun = GunPicker.SelectedItem as GunModel;
        var secilenOgun = OgunPicker.SelectedItem as OgunModel;
        var secilenTarif = TarifPicker.SelectedItem as Tarifler;

        if (secilenGun == null || secilenOgun == null || secilenTarif == null)
        {
            await DisplayAlert("Uyarý", "Lütfen tüm seçimleri yapýnýz.", "Tamam");
            return;
        }

        int gunId = secilenGun.Id;
        int ogunId = secilenOgun.Id;
        int tarifId = secilenTarif.Id;

        var zatenVarMi = HaftalikPlanlar.Any(p =>
            p.GunId == gunId &&
            p.OgunId == ogunId &&
            p.TarifId == tarifId);

        if (zatenVarMi)
        {
            await DisplayAlert("Uyarý", "Bu tarif zaten bu gün ve öðün için eklenmiþ.", "Tamam");
            return;
        }

        var yeniPlan = new HaftalikPlanModel
        {
            KullaniciId = _girisYapanKullaniciId,
            GunId = gunId,
            Gun = secilenGun.Ad,
            OgunId = ogunId,
            Ogun = secilenOgun.Ad,
            TarifId = tarifId,
            TarifAdi = secilenTarif.Ad
        };

        try
        {
            var eklenenPlan = await _haftalikPlanService.PlanEkleAsync(yeniPlan);



            if (eklenenPlan != null)
            {

                await DisplayAlert("Baþarýlý", "Plan baþarýyla eklendi.", "Tamam");

                await PlanlariYukle();

                // PlanlarTablo'yu temizle ve HaftalikPlanlar'dan tekrar doldur
                PlanlarTablo.Clear();
                foreach (var plan in HaftalikPlanlar)
                {
                    var key = (plan.Gun, plan.Ogun);
                    if (!PlanlarTablo.ContainsKey(key))
                        PlanlarTablo[key] = new List<string>();
                    PlanlarTablo[key].Add(plan.TarifAdi);
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    TabloOlustur();
                });
            }

            else
            {
                await DisplayAlert("Hata", "Plan eklenemedi.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Plan ekleme hatasý: {ex.Message}", "Tamam");
        }
    }

    private async void SilPlan_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var plan = button?.CommandParameter as HaftalikPlanModel;

        if (plan == null)
            return;

        bool onay = await DisplayAlert("Sil", $"{plan.Gun} - {plan.Ogun} öðünündeki {plan.TarifAdi} planýný silmek istiyor musunuz?", "Evet", "Hayýr");

        if (!onay)
            return;

        try
        {
            bool silindi = await _haftalikPlanService.PlanSilAsync(plan.Id);
            if (silindi)
            {
                HaftalikPlanlar.Remove(plan);

                PlanlarTablo.Clear();
                foreach (var p in HaftalikPlanlar)
                {
                    var key = (p.Gun, p.Ogun);
                    if (!PlanlarTablo.ContainsKey(key))
                        PlanlarTablo[key] = new List<string>();
                    PlanlarTablo[key].Add(p.TarifAdi);
                }

                TabloOlustur();

                await DisplayAlert("Baþarýlý", "Plan silindi.", "Tamam");
            }

            else
            {
                await DisplayAlert("Hata", "Plan silinemedi.", "Tamam");


            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Silme hatasý: {ex.Message}", "Tamam");
        }
    }

    public void TabloOlustur()
    {
        PlanTabloGrid.Children.Clear();
        PlanTabloGrid.ColumnDefinitions.Clear();
        PlanTabloGrid.RowDefinitions.Clear();

        // Öðün baþlýklarý (sütunlar)
        PlanTabloGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        for (int i = 0; i < Ogunler.Count; i++)
        {
            PlanTabloGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        // Gün baþlýklarý (satýrlar)
        PlanTabloGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int i = 0; i < Gunler.Count; i++)
        {
            PlanTabloGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // Sütun baþlýklarý
        for (int i = 0; i < Ogunler.Count; i++)
        {
            var label = new Label
            {
                Text = Ogunler[i].Ad,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };
            PlanTabloGrid.Add(label, i + 1, 0);
        }

        // Satýr baþlýklarý ve içerikler
        for (int i = 0; i < Gunler.Count; i++)
        {
            var gunLabel = new Label
            {
                Text = Gunler[i].Ad,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            PlanTabloGrid.Add(gunLabel, 0, i + 1);

            for (int j = 0; j < Ogunler.Count; j++)
            {
                var key = (Gunler[i].Ad, Ogunler[j].Ad);

                var tarifListesi = new VerticalStackLayout
                {
                    Spacing = 5,
                    Padding = new Thickness(5)
                };

                if (PlanlarTablo.ContainsKey(key))
                {
                    foreach (var tarifAdi in PlanlarTablo[key])
                    {
                        // Doðru ID'li planý bul
                        var plan = HaftalikPlanlar.FirstOrDefault(p =>
                            p.Gun == key.Item1 &&
                            p.Ogun == key.Item2 &&
                            p.TarifAdi == tarifAdi);

                        if (plan != null)
                        {
                            var tarifSatiri = new HorizontalStackLayout
                            {
                                Spacing = 5
                            };

                            var label = new Label
                            {
                                Text = "- " + tarifAdi,
                                LineBreakMode = LineBreakMode.WordWrap,
                                FontSize = 14,
                                HorizontalOptions = LayoutOptions.StartAndExpand
                            };

                            var silButton = new Button
                            {
                                Text = "Sil",
                                BackgroundColor = Colors.Red,
                                TextColor = Colors.White,
                                FontSize = 12,
                                Padding = 4,
                                CornerRadius = 5,
                                CommandParameter = plan // Artýk plan.Id burada var
                            };

                            silButton.Clicked += SilPlan_Clicked;

                            tarifSatiri.Children.Add(label);
                            tarifSatiri.Children.Add(silButton);

                            tarifListesi.Children.Add(tarifSatiri);
                        }
                    }
                }

                var scroll = new ScrollView
                {
                    Orientation = ScrollOrientation.Vertical,
                    Content = tarifListesi,
                    HeightRequest = 100
                };

                var frame = new Frame
                {
                    BorderColor = Colors.Gray,
                    CornerRadius = 5,
                    Padding = 0,
                    Content = scroll,
                    HasShadow = false,
                    Margin = 1
                };

                PlanTabloGrid.Add(frame, j + 1, i + 1);
            }
        }
    }

    public async Task PlanlariYukle()
    {
        var planList = await _haftalikPlanService.KullaniciHaftalikPlanlariniGetirAsync(_girisYapanKullaniciId);
        HaftalikPlanlar.Clear();
        foreach (var plan in planList)
        {

            HaftalikPlanlar.Add(plan);
        }
    }
}