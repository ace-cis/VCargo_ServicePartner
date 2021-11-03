using System;
using System.Net;
using VCargo_ServicePartner.Services;
using VCargo_ServicePartner.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VCargo_ServicePartner
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<UserMockDataStore>();
            DependencyService.Register<MockDataStore>();
            MainPage = new LoginPage();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
