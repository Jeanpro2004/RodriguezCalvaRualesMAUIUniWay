using Microsoft.Extensions.Logging;
using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Repositorios;
using RodriguezCalvaRualesMAUIUniWay.ViewModels;
using RodriguezCalvaRualesMAUIUniWay.Views;
using RodriguezCalvaRualesMAUIUniWay.Converters;
using RodriguezCalvaRualesMAUIUniWay.Services;

namespace RodriguezCalvaRualesMAUIUniWay;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Registrar servicios principales
        builder.Services.AddSingleton<UsuarioService>();
        builder.Services.AddSingleton<ManejoArchivosRepository>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<ViajeService>();
        builder.Services.AddSingleton<AuthenticationService>();

        // Registrar ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<MyRidesViewModel>();
        builder.Services.AddTransient<SearchRideViewModel>();

        // Registrar Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<MyRidesPage>();
        builder.Services.AddTransient<SearchRidePage>();

        // Registrar Converters
        builder.Services.AddSingleton<InvertedBoolConverter>();
        builder.Services.AddSingleton<IsNotNullConverter>();
        builder.Services.AddSingleton<IsNotNullOrEmptyConverter>();
        builder.Services.AddSingleton<DateTimeToStringConverter>();
        builder.Services.AddSingleton<BoolToColorConverter>();
        builder.Services.AddSingleton<StringToColorConverter>();
        builder.Services.AddSingleton<DecimalToCurrencyConverter>();
        builder.Services.AddSingleton<TimeSpanToStringConverter>();
        builder.Services.AddSingleton<IntToStringConverter>();
        builder.Services.AddSingleton<ListCountToVisibilityConverter>();
        builder.Services.AddSingleton<StringFormatConverter>();
        builder.Services.AddSingleton<EnumToStringConverter>();
        builder.Services.AddSingleton<ValidationErrorConverter>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}