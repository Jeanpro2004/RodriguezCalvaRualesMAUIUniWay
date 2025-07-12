using Microsoft.Extensions.Logging;
using RodriguezCalvaRualesMAUIUniWay.Services;
using RodriguezCalvaRualesMAUIUniWay.ViewModels;
using RodriguezCalvaRualesMAUIUniWay.Views;

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

        // Registrar HttpClient
        builder.Services.AddHttpClient<IUsuarioService, UsuarioService>();

        // Registrar servicios principales
        builder.Services.AddSingleton<ILogService, LogService>();
        builder.Services.AddSingleton<IFileManagementService, FileManagementService>();
        builder.Services.AddSingleton<IUserSessionService, UserSessionService>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();

        // Registrar ViewModels mejorados
        builder.Services.AddTransient<EnhancedLoginViewModel>();
        builder.Services.AddTransient<EnhancedRegisterViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Registrar páginas con ViewModels mejorados
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ProfilePage>();

        // Registrar otras páginas existentes
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<SearchRidePage>();
        builder.Services.AddTransient<MyRidesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}