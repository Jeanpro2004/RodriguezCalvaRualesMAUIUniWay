using Microsoft.Extensions.Logging;
using RodriguezCalvaRualesMAUIUniWay;
using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Repositorios;
using RodriguezCalvaRualesMAUIUniWay.Services;
using RodriguezCalvaRualesMAUIUniWay.ViewModels;
using RodriguezCalvaRualesMAUIUniWay.Views;

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

        // Registro del servicio de base de datos local - CORREGIDO
        builder.Services.AddSingleton<ReservaDatabaseLocal>(s =>
            new ReservaDatabaseLocal(Path.Combine(FileSystem.AppDataDirectory, "uniway.db3")));

        // Registrar servicios principales
        builder.Services.AddSingleton<UsuarioService>();
        builder.Services.AddSingleton<VehiculoService>();
        builder.Services.AddSingleton<ReservaService>();
        builder.Services.AddSingleton<ManejoArchivosRepository>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<ViajeService>();
        builder.Services.AddSingleton<AuthenticationService>();

        // Registrar servicios con interfaces
        builder.Services.AddSingleton<ILogService, LogService>();
        builder.Services.AddSingleton<IUserSessionService, UserSessionService>();
        builder.Services.AddSingleton<IFileManagementService, FileManagementService>();
        builder.Services.AddSingleton<IUsuarioService, UsuarioServiceImpl>();

        // Registrar ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<MyRidesViewModel>();
        builder.Services.AddTransient<SearchRideViewModel>();
        builder.Services.AddTransient<ReservasViewModel>(); // AGREGADO

        // Registrar Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<MyRidesPage>();
        builder.Services.AddTransient<SearchRidePage>();
        builder.Services.AddTransient<ReservasPage>(); // AGREGADO

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}