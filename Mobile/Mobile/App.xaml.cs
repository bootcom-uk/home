using DryIoc;
using Microsoft.Extensions.Configuration;
using Mobile.Common.Config;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System.Reflection;
using System.Text.Json;

namespace Mobile
{
    public partial class App : Application
    {
        public App(IConfiguration configuration)
        {
            InitializeComponent();

            var appSettings = configuration.Get<AppSettings>();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(appSettings.Syncfusion.LicenceKey);
        }

        protected override void OnResume()
        {
            base.OnResume();

            //var isAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);

            //if (isAvailable)
            //{
            //    var request = new AuthenticationRequestConfiguration(
            //                             "Login using biometrics",
            //                             "Confirm login with your biometrics");
            //    request.AllowAlternativeAuthentication = true;

            //    var result = await Dispatcher.DispatchAsync(async () => await CrossFingerprint.Current.AuthenticateAsync(request));

            //    if(result.Status != FingerprintAuthenticationResultStatus.Succeeded)
            //    {
            //        Application.Current.Quit();
            //    }
            //}

            
        }
    }
}
