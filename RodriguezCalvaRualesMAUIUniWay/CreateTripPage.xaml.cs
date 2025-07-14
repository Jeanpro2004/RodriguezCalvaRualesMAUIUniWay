using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Views;

public partial class CreateTripPage : ContentPage
{
    private readonly ViajeService _viajeService;

    public CreateTripPage()
    {
        InitializeComponent();
        _viajeService = new ViajeService();
        FechaPicker.Date = DateTime.Today;
        HoraPicker.Time = new TimeSpan(8, 0, 0);
    }

    private async void OnCreateTripClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(OrigenEntry.Text) ||
            string.IsNullOrWhiteSpace(DestinoEntry.Text) ||
            string.IsNullOrWhiteSpace(PrecioEntry.Text) ||
            string.IsNullOrWhiteSpace(AsientosEntry.Text))
        {
            await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
            return;
        }

        if (!decimal.TryParse(PrecioEntry.Text, out decimal precio))
        {
            await DisplayAlert("Error", "Precio inválido", "OK");
            return;
        }

        if (!int.TryParse(AsientosEntry.Text, out int asientos))
        {
            await DisplayAlert("Error", "Número de asientos inválido", "OK");
            return;
        }

        var fechaHoraSalida = FechaPicker.Date + HoraPicker.Time;

        var nuevoViaje = new Viaje
        {
            Origen = OrigenEntry.Text.Trim(),
            Destino = DestinoEntry.Text.Trim(),
            FechaHoraSalida = fechaHoraSalida,
            Precio = precio,
            AsientosDisponibles = asientos,
            ConductorId = SessionService.CurrentUserId
        };

        try
        {
            await _viajeService.CreateViajeAsync(nuevoViaje);
            await DisplayAlert("Éxito", "Viaje creado correctamente", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo crear el viaje: {ex.Message}", "OK");
        }
    }
}
