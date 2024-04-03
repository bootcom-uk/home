using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Mobile.ViewModels;
using Mobile.ViewModels.Authentication;
using Mobile.ViewModels.Budgets;
using Mobile.ViewModels.FuturePayments;
using Mobile.ViewModels.PaymentPeriods;
using Mobile.ViewModels.PaymentTypes;
using Mobile.ViewModels.Scheduling;
using Mobile.Views;
using Mobile.Views.Authentication;
using Mobile.Views.Budgets;
using Mobile.Views.FuturePayments;
using Mobile.Views.PaymentPeriods;
using Mobile.Views.PaymentTypes;
using Mobile.Views.Scheduling;
using Services;
using Services.DataServices;
using Syncfusion.Maui.Core.Hosting;
using System.Reflection;

namespace Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {


#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) => { h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent); });
            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("NoUnderline", (h, v) => { h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent); });

#endif

            var builder = MauiApp.CreateBuilder();

            var assembly = Assembly.GetExecutingAssembly();
            var stream = null as Stream;

#if DEBUG
            stream = assembly.GetManifestResourceStream("Mobile.appsettings.development.json");

#else
            stream = assembly.GetManifestResourceStream("Mobile.appsettings.json");
#endif

            builder.Configuration.AddJsonStream(stream);

            builder
                .UseMauiApp<App>()
                .UsePrism(prism =>
                {
                    prism.RegisterTypes(containerRegistry =>
                    {

                        containerRegistry
                        .RegisterScoped<HttpService>();

                        containerRegistry
                        .RegisterScoped<RealmService>();

                        containerRegistry
                        .RegisterScoped<BudgetsService>();

                        containerRegistry
                        .RegisterScoped<FuturePaymentsService>();

                        containerRegistry
                        .RegisterScoped<PaymentPeriodService>();

                        containerRegistry
                        .RegisterForNavigation<NavigationPage>();

                        containerRegistry
                        .RegisterForNavigation<LoginPage, LoginPageViewModel>()
                        .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                        .RegisterForNavigation<AuthenticateEmailPage, AuthenticateEmailPageViewModel>()
                        .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                        .RegisterForNavigation<ValidateLoginPage, ValidateLoginPageViewModel>()
                        .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                        .RegisterForNavigation<MainPage, MainPageViewModel>()
                        .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                        .RegisterForNavigation <SchedulerPage, SchedulerPageViewModel>()
                        .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                        .RegisterForNavigation<BudgetsPage, BudgetsPageViewModel>()
                        .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                        .RegisterForNavigation<PaymentPeriodsPage, PaymentPeriodsPageViewModel>()
                        .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                       .RegisterForNavigation<FuturePaymentsPage, FuturePaymentsPageViewModel>()
                       .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                      .RegisterForNavigation<PaymentTypesPage, PaymentTypesPageViewModel>()
                      .RegisterInstance(SemanticScreenReader.Default);

                        containerRegistry
                        .RegisterForNavigation<PaymentsForBudgetByPaymentPeriodPage, PaymentsForBudgetByPaymentPeriodPageViewModel>()
                        .RegisterInstance(SemanticScreenReader.Default);
                        

                    });
                    prism.OnAppStart(async navigationService =>
                    {
                        // We have a stored token
                        if (!string.IsNullOrWhiteSpace(InternalSettings.UserToken))
                        {
                            // Is this user still authenticated?
                            await navigationService.NavigateAsync("ValidateLoginPage");
                            
                            return;
                        }

                        // Take the user to a page where they can authenticate themselves
                        await navigationService.NavigateAsync("LoginPage");
                    });
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("ARLRDBD.TTF", "ArialRoundedMtBold");
                })
                .ConfigureSyncfusionCore()
                .UseMauiCommunityToolkit();


            return builder.Build();
        }
    }
}
