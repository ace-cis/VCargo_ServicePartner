using System;
using System.Collections.Generic;
using System.Net;
using VCargo_ServicePartner.ViewModels;
using VCargo_ServicePartner.Views;
using Xamarin.Forms;

namespace VCargo_ServicePartner
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
            Routing.RegisterRoute(nameof(DeliveredDetailPage), typeof(DeliveredDetailPage));
            Routing.RegisterRoute(nameof(ImageAndSignaturePage), typeof(ImageAndSignaturePage));
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

    }
}
