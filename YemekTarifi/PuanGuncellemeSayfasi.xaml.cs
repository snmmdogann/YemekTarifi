using YemekTarifi.Modeller;
using YemekTarifi.Servisler;

namespace YemekTarifi;

public partial class PuanGuncellemeSayfasi : ContentPage
{
    private PuanModel _puan;
    private IPuanService _puanService;
    public PuanGuncellemeSayfasi(PuanModel puan)
    {
        InitializeComponent();
        _puan = puan;
        _puanService = new PuanService();

        YorumEditor.Text = _puan.Yorum;
        PuanEntry.Text = _puan.Deger.ToString();
    }

    private async void GuncelleButton_Clicked(object sender, EventArgs e)
    {
        if (!int.TryParse(PuanEntry.Text, out int puanDegeri))
        {
            await DisplayAlert("Uyarý", "Lütfen geçerli bir puan giriniz.", "Tamam");
            return;
        }

        _puan.Deger = puanDegeri;
        _puan.Yorum = YorumEditor.Text;

        if (!_puan.IsValid())
        {
            string mesaj = string.Join("\n", _puan.ValidationErrors);
            await DisplayAlert("Uyarý", mesaj, "Tamam");
            return;
        }

        bool guncellemeSonuc = await _puanService.GuncellePuanAsync(_puan);

        if (guncellemeSonuc)
        {
            await DisplayAlert("Baþarýlý", "Yorumunuz güncellendi.", "Tamam");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Hata", "Yorum güncellenemedi.", "Tamam");
        }
    }

}