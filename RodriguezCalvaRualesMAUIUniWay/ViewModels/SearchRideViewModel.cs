using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class SearchRideViewModel : BaseViewModel
    {
        private readonly ViajeService _viajeService;
        private string _origin = string.Empty;
        private string _destination = string.Empty;
        private DateTime _travelDate = DateTime.Today;
        private string _selectedPassengers = "1";

        public SearchRideViewModel(ViajeService viajeService)
        {
            _viajeService = viajeService;
            Title = "Buscar Viaje";
            SearchCommand = new Command(async () => await ExecuteSearchCommand());
            RouteSelectedCommand = new Command<string>(OnRouteSelected);

            PopularRoutes = new ObservableCollection<string>
            {
                "UDLA PARK → Centro Histórico",
                "UDLA GRANADOS → Cumbayá",
                "Universidad Central → El Recreo",
                "PUCE → La Carolina"
            };

            PassengerOptions = new ObservableCollection<string> { "1", "2", "3", "4" };
        }

        public string Origin
        {
            get => _origin;
            set => SetProperty(ref _origin, value);
        }

        public string Destination
        {
            get => _destination;
            set => SetProperty(ref _destination, value);
        }

        public DateTime TravelDate
        {
            get => _travelDate;
            set => SetProperty(ref _travelDate, value);
        }

        public string SelectedPassengers
        {
            get => _selectedPassengers;
            set => SetProperty(ref _selectedPassengers, value);
        }

        public ObservableCollection<string> PopularRoutes { get; }
        public ObservableCollection<string> PassengerOptions { get; }

        public ICommand SearchCommand { get; }
        public ICommand RouteSelectedCommand { get; }

        private async Task ExecuteSearchCommand()
        {
            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Origin) || string.IsNullOrWhiteSpace(Destination))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Por favor completa origen y destino", "OK");
                    return;
                }

                var viajes = await _viajeService.BuscarViajesAsync(Origin, Destination, TravelDate);

                // Aquí mostrarías los resultados
                await Application.Current.MainPage.DisplayAlert("Búsqueda", $"Encontrados {viajes.Count} viajes", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnRouteSelected(string route)
        {
            if (!string.IsNullOrEmpty(route))
            {
                var parts = route.Split(" → ");
                if (parts.Length == 2)
                {
                    Origin = parts[0];
                    Destination = parts[1];
                }
            }
        }
    }
}