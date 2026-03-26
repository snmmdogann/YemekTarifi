using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YemekTarifi.Modeller;
//using YemekTarifi.Service; // IUserService ve IPuanService için
using Microsoft.Maui.Controls; // DependencyService için
using YemekTarifi.Servisler;

namespace YemekTarifi
{
    public partial class TariflerimSayfasi : ContentPage
    {
        private IKullaniciService _kullaniciServisi;
        private IPuanService _puanServisi;
        private ITarifService _tarifService;

        private async void GeriDon(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Önceki sayfaya döner
        }

        private List<Tarifler> tariflerCollection = new List<Tarifler>();

        private string _girisYapanKullaniciAdi;

        public TariflerimSayfasi()
        {
            InitializeComponent();

            _kullaniciServisi = DependencyService.Get<IKullaniciService>();


            _puanServisi = new PuanService();
            _tarifService = new TarifService(); 
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();


            var animasyonKontrolleri = new VisualElement[]
            {
                btnGeriDon,
                TariflerListView

            };

            foreach (var kontrol in animasyonKontrolleri)
            {
                kontrol.Opacity = 0;
                kontrol.TranslationX = 300;
            }

            foreach (var kontrol in animasyonKontrolleri)
            {
                await Task.WhenAll(
                    kontrol.FadeTo(1, 600, Easing.CubicOut),
                    kontrol.TranslateTo(0, 0, 600, Easing.CubicOut)
                );
            }

            await KullaniciBilgisiniAlVeTarifleriYukle();
        }
        private async Task KullaniciBilgisiniAlVeTarifleriYukle()
        {
            var kullanici = await _kullaniciServisi.GetLoggedInUserAsync();

            if (kullanici != null)
            {
                _girisYapanKullaniciAdi = kullanici.KullaniciAdi;
                await TarifleriGetirVeListele();
            }
            else
            {
                await DisplayAlert("Hata", "Kullanıcı bilgisi bulunamadı!", "Tamam");
            }
        }

        private async Task TarifleriGetirVeListele()
        {
            try
            {
                var tumTarifler = await _tarifService.GetirTariflerAsync();


                if (tumTarifler == null)
                {
                    await DisplayAlert("Hata", "Tarifler boş döndü.", "Tamam");
                    return;
                }

                tariflerCollection = tumTarifler.FindAll(t => t.EkleyenKullanici == _girisYapanKullaniciAdi);

                foreach (var tarif in tariflerCollection)
                {
                    tarif.ResimUrl = _tarifService.ResimUrlTamamla(tarif.ResimYolu);


                    var yorumlar = await _puanServisi.TarifPuanlariniGetirAsync(tarif.Id);

                    if (yorumlar != null)
                    {
                        foreach (var yorum in yorumlar)
                        {
                            var kullanici = await _kullaniciServisi.GetUserByIdAsync(yorum.KullaniciId.ToString());
                            yorum.KullaniciAdi = kullanici?.KullaniciAdi ?? "Bilinmiyor";
                        }
                    }

                    tarif.Yorum = yorumlar ?? new List<PuanModel>();
                }

                TariflerListView.ItemsSource = tariflerCollection;
            }
            catch (Exception hata)
            {
                await DisplayAlert("Hata", $"Tarifler yüklenemedi: {hata.Message}", "Tamam");
            }
        }



        private async void EkleTarif_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TarifEklemeSayfasi());
        }

        private async void SilTarif_Clicked(object sender, EventArgs e)
        {
            var buton = (Button)sender;
            var secilenTarif = buton.CommandParameter as Tarifler;

            if (secilenTarif == null)
                return;

            bool onay = await DisplayAlert("Sil", $"{secilenTarif.Ad} tarifini silmek istiyor musunuz?", "Evet", "Hayır");

            if (onay)
            {
                try
                {
                    bool sonuc = await _tarifService.SilTarifAsync(secilenTarif.Id);

                    if (sonuc)
                    {
                        tariflerCollection.Remove(secilenTarif);
                        TariflerListView.ItemsSource = null;
                        TariflerListView.ItemsSource = tariflerCollection;

                        await DisplayAlert("Başarılı", "Tarif silindi.", "Tamam");
                    }
                    else
                    {
                        await DisplayAlert("Hata", "Tarif silinemedi.", "Tamam");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Hata", $"Hata oluştu: {ex.Message}", "Tamam");
                }
            }
        }

        private async void OnGuncelleButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var secilenTarif = button?.BindingContext as Tarifler;

            if (secilenTarif != null)
            {
                await Navigation.PushAsync(new TarifGuncellemeSayfasi(secilenTarif));
            }
        }
        private async void TariflerListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var secilenTarif = e.SelectedItem as Tarifler;
            if (secilenTarif != null)
            {
                await Navigation.PushAsync(new TariflerSayfasi(secilenTarif.Id, secilenTarif.Ad));

                ((ListView)sender).SelectedItem = null;
            }
        }



    }

}