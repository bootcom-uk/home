using Microsoft.Extensions.Configuration;
using Mobile.Common.Config;
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
    }
}
