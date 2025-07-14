using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class MyRidesViewModel : BaseViewModel
    {
        private readonly ViajeService _viajeService;
        private readonly AuthenticationService _authService;
        private Usuario _usuarioActual;
        private bool _hasRides = false;

        public MyRidesViewModel(ViajeService viajeService, AuthenticationService authService)
        {
            _viajeService = viajeService;
            _authService = authService;
            Title = "Mis Viajes";

            MisViajes = new ObservableCollection<ViajeModel>();
            ViajesDisponibles = new ObservableCollection<ViajeModel>();

            RefreshCommand = new Command(async () => await ExecuteRefreshCommand());
            CreateRideCommand = new Command(async () => await ExecuteCreateRideCommand());
        }

        public Usuario UsuarioActual
        {
            get => _usuarioActual;
            set => SetProperty(ref _usuarioActual, value);
        }

        public bool HasRides
        {
            get => _hasRides;
            set => SetProperty(ref _hasRides, value);
        }

        public ObservableCollection<ViajeModel> MisViajes { get; }
        public ObservableCollection<ViajeModel> ViajesDisponibles { get; }

        public ICommand RefreshCommand { get; }
        public ICommand CreateRideCommand { get; }

        public async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                UsuarioActual = await _authService.GetCurrentUserAsync();

                if (UsuarioActual != null)
                {
                    if (UsuarioActual.EsConductor)
                    {
                        var misViajes = await _viajeService.GetViajesPorConductorAsync(UsuarioActual.Id);
                        MisViajes.Clear();
                        foreach (var viaje in misViajes)
                        {
                            MisViajes.Add(viaje);
                        }
                    }
                    else
                    {
                        var viajesDisponibles = await _viajeService.GetViajesAsync();
                        ViajesDisponibles.Clear();
                        foreach (var viaje in viajesDisponibles.Where(v => v.EspaciosDisponibles > 0))
                        {
                            ViajesDisponibles.Add(viaje);
                        }
                    }

                    HasRides = MisViajes.Any() || ViajesDisponibles.Any();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteRefreshCommand()
        {
            await LoadDataAsync();
        }

        private async Task ExecuteCreateRideCommand()
        {
            await Application.Current.MainPage.DisplayAlert("Crear Viaje", "Funcionalidad próximamente", "OK");
        }
    }
}