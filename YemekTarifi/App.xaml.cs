using Microsoft.Maui.Controls;

using YemekTarifi.Servisler;
using YemekTarifi.Modeller;
namespace YemekTarifi
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Servisi kaydet
            DependencyService.Register<IKullaniciService, KullaniciServisi>();

            // Mevcut sayfanı koru, örneğin giriş sayfası
            MainPage = new NavigationPage(new KullaniciGirisSayfasi());
        }
    }
}
