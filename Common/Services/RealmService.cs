using Microsoft.Extensions.Configuration;
using Models;
using Realms.Sync;
using Services.Config;
using System.Reflection;
using System.Text.Json;

namespace Services
{
    public class RealmService(IConfiguration configuration)
    {

        private Realms.Realm _realm;

        public Realms.Realm Realm => _realm;

        public async Task InitializeAsync()
        {
            var appSettings = configuration.Get<AppSettings>();

            var app = Realms.Sync.App.Create(new AppConfiguration(appSettings!.RealmDetails.AppId));

            var usernamePasswordCreds = Credentials.EmailPassword(appSettings!.RealmDetails.EmailAddress, appSettings!.RealmDetails.Password);

            var user = await app.LogInAsync(usernamePasswordCreds);

            var config = new FlexibleSyncConfiguration(app.CurrentUser!);
            config.Schema = new[] { typeof(PaymentType), typeof(Payments), typeof(BudgetCategories), typeof(PaymentCategory), typeof(PaymentPeriod), typeof(PaymentPeriod_Budgets), typeof(FuturePayment), typeof(Models.User) };

            // config.EncryptionKey = Encoding.ASCII.GetBytes("iAaTxFA0gV9rwd8mD0tJHLQd0kLAFsiXSemKkYPUTqfinMw4hN3yJX16k23PW7bQ");

            _realm = Realms.Realm.GetInstance(config);

            var path = _realm.Config.DatabasePath;

            _realm.Subscriptions.Update(() =>
            {
                _realm.Subscriptions.Add(_realm.All<PaymentPeriod>(), new() { Name = "Payment Periods" });
                _realm.Subscriptions.Add(_realm.All<PaymentCategory>(), new() { Name = "Payment Categories" });
                _realm.Subscriptions.Add(_realm.All<BudgetCategories>(), new() { Name = "Budget Categories" });
                _realm.Subscriptions.Add(_realm.All<PaymentType>(), new() { Name = "Payment Types" });
                _realm.Subscriptions.Add(_realm.All<Payments>(), new() { Name = "Payments" });
                _realm.Subscriptions.Add(_realm.All<FuturePayment>(), new() { Name = "Future Payments" });
                _realm.Subscriptions.Add(_realm.All<Models.User>(), new() { Name = "User" });
            });

            try
            {
                await _realm.Subscriptions.WaitForSynchronizationAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

        }

    }
}
