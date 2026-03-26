using System.Text.Json;
using YemekTarifi.Modeller;
using YemekTarifi.Servisler;


namespace YemekTarifi;

public partial class KullaniciGirisSayfasi : ContentPage
{
    private KullaniciServisi _kullaniciServisi;

    private string secilenCinsiyet = null;

    public KullaniciGirisSayfasi()
    {
        InitializeComponent();
        BindingContext = this;
        _kullaniciServisi = new KullaniciServisi();
    }

    private void GirisSekmesi_Clicked(object sender, EventArgs e)
    {
        girisLayout.IsVisible = true;
        uyeOlLayout.IsVisible = false;
    }

    private void UyeOlSekmesi_Clicked(object sender, EventArgs e)
    {
        girisLayout.IsVisible = false;
        uyeOlLayout.IsVisible = true;
    }

    private bool sifreGoster = false;

    private void BtnSifreGoster_Clicked(object sender, EventArgs e)
    {
        sifreGoster = !sifreGoster;
        sifreEntry.IsPassword = !sifreGoster;

        btnSifreGoster.Text = sifreGoster ? "🙈" : "👁";
    }

    bool uyeSifreGoster = false;
    bool uyeSifreTekrarGoster = false;

    private void BtnUyeSifreGoster_Clicked(object sender, EventArgs e)
    {
        uyeSifreGoster = !uyeSifreGoster;
        uyeSifreEntry.IsPassword = !uyeSifreGoster;
        btnUyeSifreGoster.Text = uyeSifreGoster ? "🙈" : "👁";
    }

    private void BtnUyeSifreTekrarGoster_Clicked(object sender, EventArgs e)
    {
        uyeSifreTekrarGoster = !uyeSifreTekrarGoster;
        uyeSifreTekrarEntry.IsPassword = !uyeSifreTekrarGoster;
        btnUyeSifreTekrarGoster.Text = uyeSifreTekrarGoster ? "🙈" : "👁";
    }



    private void Cinsiyet_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var rb = sender as RadioButton;
        if (rb.IsChecked)
        {
            secilenCinsiyet = rb.Content.ToString();

        }
    }


    private async void GirisYap_Clicked(object sender, EventArgs e)
    {
        string kullaniciAdi = kullaniciAdiEntry.Text;
        string sifre = sifreEntry.Text;


        if (string.IsNullOrWhiteSpace(kullaniciAdi))
        {
            await DisplayAlert("Hata", "Lütfen kullanıcı adınızı girin", "Tamam");
            return;
        }

        if (string.IsNullOrWhiteSpace(sifre))
        {
            await DisplayAlert("Hata", "Lütfen şifrenizi girin", "Tamam");
            return;
        }

        var girisModel = new KullaniciGirisModel
        {
            KullaniciAdi = kullaniciAdi,
            Sifre = sifre
        };

        var sonuc = await _kullaniciServisi.GirisYapAsync(girisModel);

        if (sonuc != null)
        {
            string kullaniciJson = JsonSerializer.Serialize(sonuc);
            Preferences.Default.Set("GirisYapanKullanici", kullaniciJson);
            Preferences.Default.Set("KullaniciAdi", sonuc.KullaniciAdi);

            await DisplayAlert("Başarılı", "Giriş başarılı!", "Tamam");
            await Task.Delay(2000);
            await Navigation.PushAsync(new KategoriAnaSayfa());
        }
        else
        {
            await DisplayAlert("Hata", "Kullanıcı adı veya şifre yanlış.", "Tamam");
        }
    }


    private bool _telefonFormatlamaYapiliyor = false;

    private void uyeTelefonEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_telefonFormatlamaYapiliyor)
            return;

        _telefonFormatlamaYapiliyor = true;

        var entry = sender as Entry;
        string text = entry.Text ?? "";

        // Sayı dışındaki karakterleri kaldır
        string digits = new string(text.Where(char.IsDigit).ToArray());

        // En fazla 11 hane (0 + 10 hane) al
        if (digits.Length > 11)
            digits = digits.Substring(0, 11);

        // Format oluştur
        string formatted = "";

        if (digits.Length > 0)
        {
            formatted += digits[0]; // 0

            if (digits.Length > 1)
            {
                formatted += " (";

                if (digits.Length >= 4)
                {
                    formatted += digits.Substring(1, 3) + ") ";
                    if (digits.Length >= 7)
                    {
                        formatted += digits.Substring(4, 3) + " ";
                        if (digits.Length >= 9)
                        {
                            formatted += digits.Substring(7, 2) + " ";
                            if (digits.Length >= 11)
                            {
                                formatted += digits.Substring(9, 2);
                            }
                            else if (digits.Length > 9)
                            {
                                formatted += digits.Substring(9);
                            }
                        }
                        else if (digits.Length > 7)
                        {
                            formatted += digits.Substring(7);
                        }
                    }
                    else if (digits.Length > 4)
                    {
                        formatted += digits.Substring(4);
                    }
                }
                else
                {
                    formatted += digits.Substring(1);
                }
            }
        }

        entry.Text = formatted;
        _telefonFormatlamaYapiliyor = false;
    }

    private async void UyeOl_Clicked(object sender, EventArgs e)
    {
        if (uyeSifreEntry.Text != uyeSifreTekrarEntry.Text)
        {
            await DisplayAlert("Hata", "Şifreler aynı olmalı!", "Tamam");
            return; 
        }
        var kayitModel = new KullaniciUyeOlModel
        {
            KullaniciAdi = uyeKullaniciAdiEntry.Text,
            KullaniciSoyadi = uyeSoyadiEntry.Text,
            Sifre = uyeSifreEntry.Text,
            SifreTekrar = uyeSifreTekrarEntry.Text,
            Cinsiyet = secilenCinsiyet,
            DogumTarihi = uyeDogumTarihiPicker.Date.ToString("yyyy-MM-dd"),
            Eposta = uyeEpostaEntry.Text,
            Telefon = uyeTelefonEntry.Text,
            KayitTarihi = DateTime.Now
        };

        bool result = await _kullaniciServisi.UyeOlAsync(kayitModel);

        if (result)
        {
            await DisplayAlert("Başarılı", "Üyelik başarılı!", "Tamam");
            await Task.Delay(2000);
            // Üye ol sayfasından giriş formuna dön
            uyeOlLayout.IsVisible = false;
            girisLayout.IsVisible = true;
        }
        else
        {
            await DisplayAlert("Hata", "Bir hata oluştu. Lütfen tekrar deneyin.", "Tamam");
        }
    }

    private void GirisEkraninaDon_Clicked(object sender, EventArgs e)
    {
        // Üye ol formunu gizle, giriş formunu göster
        uyeOlLayout.IsVisible = false;
        girisLayout.IsVisible = true;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var animasyonluKontroller = new VisualElement[]
        {
            girisLayout,
            uyeOlLayout,
            kullaniciAdiEntry,
            sifreEntry,
            btnSifreGoster,
            uyeKullaniciAdiEntry,
            uyeSoyadiEntry,
            uyeSifreEntry,
            uyeSifreTekrarEntry,
            uyeEpostaEntry,
            uyeDogumTarihiPicker,
            uyeTelefonEntry,
            btnUyeSifreGoster,
            btnUyeSifreTekrarGoster
        };

        foreach (var kontrol in animasyonluKontroller)
        {
            kontrol.Opacity = 0;
            kontrol.TranslationX = 300;
        }

        foreach (var kontrol in animasyonluKontroller)
        {
            await Task.WhenAll(
                kontrol.FadeTo(1, 600, Easing.CubicOut),
                kontrol.TranslateTo(0, 0, 600, Easing.CubicOut)
            );
        }
    }
}