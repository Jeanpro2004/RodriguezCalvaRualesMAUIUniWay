// MauiProgram.cs
using Microsoft.Extensions.Logging;
using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Services;
using RodriguezCalvaRualesMAUIUniWay.ViewModels;
using RodriguezCalvaRualesMAUIUniWay.Views;
using RodriguezCalvaRualesMAUIUniWay.Interfaces;

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

        // Registrar base de datos SQLite
        builder.Services.AddSingleton<ReservaDatabaseService>(s =>
            new ReservaDatabaseService(Path.Combine(FileSystem.AppDataDirectory, "reservas.db")));

        // Registrar servicios de archivos y login attempts
        builder.Services.AddSingleton<IFileService, FileService>();
        builder.Services.AddSingleton<ILoginAttemptService, LoginAttemptService>();

        // Registrar servicios API
        builder.Services.AddSingleton<UsuarioService>();
        builder.Services.AddSingleton<VehiculoService>();
        builder.Services.AddSingleton<ViajeService>();
        builder.Services.AddSingleton<ReservaService>();

        // Registrar ViewModels
        builder.Services.AddTransient<SearchRideViewModel>();
        builder.Services.AddTransient<ReservasViewModel>();

        // Registrar Views
        builder.Services.AddTransient<SearchRidePage>();
        builder.Services.AddTransient<ReservasPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}