using RodriguezCalvaRualesMAUIUniWay.API;
using System.Collections.ObjectModel;

namespace RodriguezCalvaRualesMAUIUniWay.Views
{
    public partial class MyRidesPage : ContentPage
    {
        private readonly ReservaService _reservaService = new();
        private readonly ViajeService _viajeService = new();

        // This will hold the combined Reserva + Viaje info
        public ObservableCollection<RideViewModel> MyRides { get; set; } = new();

        public MyRidesPage()
        {
            InitializeComponent();
            LoadMyRides();
            MyRidesCollectionView.ItemsSource = MyRides;
        }

        private async void LoadMyRides()
        {
            try
            {
                int userId = SessionService.CurrentUserId;

                // Get all reservas
                var allReservas = await _reservaService.GetReservasAsync();

                // Filter only reservas for this user
                var myReservas = allReservas.Where(r => r.PasajeroId == userId).ToList();

                MyRides.Clear();

                foreach (var reserva in myReservas)
                {
                    // Get viaje info for each reserva
                    var viaje = await _viajeService.GetViajeByIdAsync(reserva.ViajeId);

                    if (viaje != null)
                    {
                        MyRides.Add(new RideViewModel
                        {
                            Origen = viaje.Origen,
                            Destino = viaje.Destino,
                            FechaHoraSalida = viaje.FechaHoraSalida,
                            Estado = reserva.Estado.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo cargar tus viajes: {ex.Message}", "OK");
            }
        }
    }

    public class RideViewModel
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public string Estado { get; set; }
    }
}
