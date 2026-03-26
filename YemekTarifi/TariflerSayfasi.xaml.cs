
using YemekTarifi.Modeller;
using YemekTarifi.Servisler;
namespace YemekTarifi;

public partial class TariflerSayfasi : ContentPage
{

    private int _tarifId;
    private string _tarifAdi;

    private int aktifKullaniciId;
    private int secilenPuan = 0;

    private PuanService _puanService = new PuanService();
    private IKullaniciService _kullaniciService;
    private ITarifService _tarifService;

    private FavoriService _favoriService = new FavoriService();

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Önceki sayfaya döner
    }
    public TariflerSayfasi(int id, string ad)
    {
        InitializeComponent();
        BindingContext = this;

        _tarifService = new TarifService();
        _tarifId = id;
        _tarifAdi = ad;

        _kullaniciService = new KullaniciServisi();

        _ = TarifiGoster();
        _ = PuanGoster();

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var kullanici = await _kullaniciService.GetLoggedInUserAsync();
        if (kullanici != null)
        {
            aktifKullaniciId = kullanici.KullaniciId;
        }
        else
        {
            await DisplayAlert("Hata", "Giriþ yapan kullanýcý bulunamadý.", "Tamam");
        }
    }
    //3.6*2=7.2 7.2 yi 7 hye yuvarla 7/2=3.5
    private double OrtalamaPuaniYuvarla(double ortalama)
        => Math.Round(ortalama * 2, MidpointRounding.AwayFromZero) / 2;

    private void YildizGorselleriniOlustur(double ortalama)
    {
        double yuvarlanmis = OrtalamaPuaniYuvarla(ortalama);
        int tamYildiz = (int)yuvarlanmis;//int türünde yazýp virgülden sonrasý gider.
        bool yarimYildizVar = yuvarlanmis - tamYildiz == 0.5;

        YildizGorselStack.Children.Clear();

        for (int i = 0; i < 5; i++)
        {
            string imageSource = i < tamYildiz ? "doluyildiz.png" :
                                 i == tamYildiz && yarimYildizVar ? "yarimyildiz.jpg" :
                                 "bosyildiz.png";

            var yildiz = new Image
            {
                Source = imageSource,
                WidthRequest = 24,
                HeightRequest = 24,
                Aspect = Aspect.AspectFit
            };

            YildizGorselStack.Children.Add(yildiz);
        }
    }

    private async Task TarifiGoster()
    {
        
        try
        {

            var tarif = await _tarifService.GetirTarifByIdAsync(_tarifId);

            if (tarif != null)
            {
                lblTarifAdi.Text = tarif.Ad;
                lblMalzemeler.Text = tarif.Malzemeler;
                lblYapilis.Text = tarif.Yapilis;
                lblEklenmeTarihi.Text = tarif.EklenmeTarihi.ToString("dd MMMM yyyy");
                lblGuncellenmeTarihi.Text = tarif.GuncellenmeTarihi.ToString("dd MMMM yyyy");
                lblEkleyenKullanici.Text = tarif.EkleyenKullanici;
                lblPisirmeSuresi.Text = tarif.PisirmeSuresi.ToString();
                lblHazirlaMesaji.Text = tarif.Hazirla();

                // Resim URL varsa görseli göster
                if (!string.IsNullOrEmpty(tarif.ResimYolu))
                {
                    string resimUrl = tarif.ResimYolu.StartsWith("http") ? tarif.ResimYolu : $"https://localhost:7213/{tarif.ResimYolu}";

                    imgTarifResim.Source = ImageSource.FromUri(new Uri(resimUrl));
                }
                else
                {

                    imgTarifResim.Source = null; // Resim yoksa boþ býrak
                }
            }
            else
            {
                await DisplayAlert("Bilgi", "Tarif bulunamadý.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Tarif gösterilemedi: {ex.Message}", "Tamam");
        }
    }


    private async Task PuanGoster()
    {
        try
        {
            var puanlar = await _puanService.TarifPuanlariniGetirAsync(_tarifId);

            if (puanlar != null && puanlar.Count > 0)
            {
                yorumlarCollectionView.ItemsSource = puanlar;

                double ortalama = puanlar.Average(p => p.Deger);
                lblPuanDeger.Text = ortalama.ToString("0.0");
                YildizGorselleriniOlustur(ortalama);
            }
            else
            {
                lblPuanDeger.Text = "Henüz puan verilmemiþ";
                lblPuanDeger.FontSize = 20;
                lblPuanYorum.Text = "Henüz yorum yok";
                YildizGorselleriniOlustur(0);
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Bilgi", "Bu tarife hiç puan verilmemiþ", "Tamam");
        }
    }


    public void YildizGoster(int puan)
    {
        var stars = new[] { star1, star2, star3, star4, star5 };

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].Source = i < puan ? "doluyildiz.png" : "bosyildiz.png";
        }
    }


    private async void BtnPuanYorumGonder_Clicked(object sender, EventArgs e)
    {
        var yorumMetni = yorumEditor.Text;

        if (secilenPuan == 0)
        {
            await DisplayAlert("Uyarý", "Lütfen puan veriniz.", "Tamam");
            return;
        }

        if (string.IsNullOrWhiteSpace(yorumMetni))
        {
            await DisplayAlert("Uyarý", "Lütfen yorum yazýnýz.", "Tamam");
            return;
        }

        var yeniPuan = new PuanModel
        {
            TarifId = _tarifId,
            KullaniciId = aktifKullaniciId,
            Deger = secilenPuan,
            Yorum = yorumMetni
        };

        var basarili = await _puanService.PuanGonderAsync(yeniPuan);

        if (basarili)
        {
            await DisplayAlert("Baþarýlý", "Puan ve yorum gönderildi", "Tamam");
            yorumEditor.Text = string.Empty;
            secilenPuan = 0;
            YildizGoster(0);
            await PuanGoster();
        }
        else
        {
            await DisplayAlert("Bilgi", "Bu tarifin hiç yorumu yok", "Tamam");
        }
    }

    private async void FavoriEkleTiklandi(object sender, EventArgs e)
    {
        try
        {
            var favori = new FavoriModel
            {
                KullaniciId = aktifKullaniciId,
                TarifId = _tarifId,
                TarifAdi = _tarifAdi,
                EklenmeTarihi = DateTime.Now
            };

            var sonuc = await _favoriService.EkleFavoriAsync(favori);

            if (sonuc)
            {
                await DisplayAlert("Baþarýlý", "Tarif favorilere eklendi!", "Tamam");
            }
            else
            {
                await DisplayAlert("Hata", "Zaten favorilerinize daha önce eklediniz", "Tamam");
            }
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Bilgi", ex.Message, "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluþtu: {ex.Message}", "Tamam");
        }
    }

    // Yýldýzlara týklanma eventleri - puan seçimi
    private void Star1_Tapped(object sender, EventArgs e) => YildizSec(1);
    private void Star2_Tapped(object sender, EventArgs e) => YildizSec(2);
    private void Star3_Tapped(object sender, EventArgs e) => YildizSec(3);
    private void Star4_Tapped(object sender, EventArgs e) => YildizSec(4);
    private void Star5_Tapped(object sender, EventArgs e) => YildizSec(5);

    private void YildizSec(int puan)
    {
        secilenPuan = puan;
        YildizGoster(puan);
    }

}