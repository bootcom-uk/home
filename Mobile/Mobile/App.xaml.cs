using Mobile.Common.Config;
using System.Reflection;
using System.Text.Json;

namespace Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            var appSettingsStream = assembly.GetManifestResourceStream("Mobile.appsettings.json");
            var appSettings = JsonSerializer.Deserialize<AppSettings>(appSettingsStream!);

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(appSettings.Syncfusion.LicenceKey);
        }
    }
}
