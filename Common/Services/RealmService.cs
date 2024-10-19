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

        public Realms.Realm? Realm { get; internal set; }

        public async Task InitializeAsync()
        {
            var appSettings = configuration.Get<AppSettings>();

            var app = Realms.Sync.App.Create(new AppConfiguration(appSettings!.RealmDetails.AppId));

            var usernamePasswordCreds = Credentials.EmailPassword(appSettings!.RealmDetails.EmailAddress, appSettings!.RealmDetails.Password);

            var user = await app.LogInAsync(usernamePasswordCreds);

            var config = new FlexibleSyncConfiguration(app.CurrentUser!);
            config.Schema = new[] { typeof(PaymentType), typeof(Payments), typeof(BudgetCategories), typeof(PaymentCategory), typeof(PaymentPeriod), typeof(PaymentPeriod_Budgets), typeof(FuturePayment), typeof(Models.User), typeof(PaymentType_PaymentSchedule) };

            Realm = Realms.Realm.GetInstance(config);

            var path = Realm.Config.DatabasePath;

            Realm.Subscriptions.Update(() =>
            {
                Realm.Subscriptions.Add(Realm.All<PaymentPeriod>(), new() { Name = "Payment Periods" });
                Realm.Subscriptions.Add(Realm.All<PaymentCategory>(), new() { Name = "Payment Categories" });
                Realm.Subscriptions.Add(Realm.All<BudgetCategories>(), new() { Name = "Budget Categories" });
                Realm.Subscriptions.Add(Realm.All<PaymentType>(), new() { Name = "Payment Types" });
                Realm.Subscriptions.Add(Realm.All<Payments>(), new() { Name = "Payments" });
                Realm.Subscriptions.Add(Realm.All<FuturePayment>(), new() { Name = "Future Payments" });
                Realm.Subscriptions.Add(Realm.All<Models.User>(), new() { Name = "User" });
            });

            try
            {
                await Realm.Subscriptions.WaitForSynchronizationAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

        }

    }
}
