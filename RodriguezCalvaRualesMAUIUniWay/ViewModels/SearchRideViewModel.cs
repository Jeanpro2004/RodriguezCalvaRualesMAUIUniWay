using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Accelerate;
using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class SearchRideViewModel : INotifyPropertyChanged
    {
        private readonly ViajeService _viajeService;
        private readonly ReservaDatabaseLocal _reservaDatabase;
        private readonly IUserSessionService _userSession;

        private bool _isBusy;
        private string _origin;
        private string _destination;
        private DateTime _travelDate = DateTime.Today;
        private int _selectedPassengers = 1;
        private string _filtroActivo = "Todos";
        private List<Viaje> _todosLosViajes = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Viaje> ViajesDisponibles { get; set; }
        public ObservableCollection<string> PassengerOptions { get; set; }
        public ObservableCollection<string> PopularRoutes { get; set; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
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

        public int SelectedPassengers
        {
            get => _selectedPassengers;
            set => SetProperty(ref _selectedPassengers, value);
        }

        public string FiltroActivo
        {
            get => _filtroActivo;
            set
            {
                SetProperty(ref _filtroActivo, value);
                AplicarFiltro();
            }
        }

        // Comandos
        public ICommand SearchCommand { get; }
        public ICommand RouteSelectedCommand { get; }
        public ICommand ReservarCommand { get; }
        public ICommand LoadViajesCommand { get; }

        public SearchRideViewModel(ViajeService viajeService, ReservaDatabaseLocal reservaDatabase, IUserSessionService userSession)
        {
            _viajeService = viajeService;
            _reservaDatabase = reservaDatabase;
            _userSession = userSession;

            ViajesDisponibles = new ObservableCollection<Viaje>();
            PassengerOptions = new ObservableCollection<string> { "1", "2", "3", "4", "5", "6" };
            PopularRoutes = new ObservableCollection<string>
            {
                "UDLA → Quicentro Sur",
                "Cumbayá → La Carolina",
                "Tumbaco → El Ejido",
                "Sangolquí → Centro Histórico"
            };

            SearchCommand = new Command(async () => await BuscarViajesAsync());
            RouteSelectedCommand = new Command<string>(async (ruta) => await SeleccionarRutaAsync(ruta));
            ReservarCommand = new Command<Viaje>(async (viaje) => await ReservarViajeAsync(viaje));
            LoadViajesCommand = new Command(async () => await CargarTodosLosViajesAsync());

            _ = CargarTodosLosViajesAsync();
        }

        public async Task CargarTodosLosViajesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var viajesApi = await _viajeService.GetViajesAsync();

                _todosLosViajes = viajesApi
                    .Select(v => new ViajeModel
                    {
                        Id = v.Id,
                        Origen = v.Origen,
                        Destino = v.Destino,
                        Fecha = v.FechaHoraSalida.Date,
                        Hora = v.FechaHoraSalida.TimeOfDay,
                        Precio = v.Precio,
                        EspaciosDisponibles = v.AsientosDisponibles,
                        EspaciosTotales = v.AsientosDisponibles, // si no tienes dato real de total, puedes dejar igual
                        ConductorId = v.ConductorId,
                        Estado = "Activo", // puedes cambiar según lógica
                        Conductor = ""     // podrías llenarlo luego con otro servicio si lo necesitas
                    })
                    .ToList();



                var viajesFiltrados = _todosLosViajes
                    .Where(v => v.FechaHoraSalida >= DateTime.Now && v.AsientosDisponibles > 0)
                    .OrderBy(v => v.FechaHoraSalida)
                    .ToList();

                ViajesDisponibles.Clear();
                foreach (var viaje in viajesFiltrados)
                {
                    ViajesDisponibles.Add(viaje);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error al cargar viajes: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task BuscarViajesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var viajesFiltrados = _todosLosViajes.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(Origin))
                {
                    viajesFiltrados = viajesFiltrados.Where(v =>
                        v.Origen.Contains(Origin, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(Destination))
                {
                    viajesFiltrados = viajesFiltrados.Where(v =>
                        v.Destino.Contains(Destination, StringComparison.OrdinalIgnoreCase));
                }

                viajesFiltrados = viajesFiltrados.Where(v =>
                    v.FechaHoraSalida.Date == TravelDate.Date);

                viajesFiltrados = viajesFiltrados.Where(v =>
                    v.AsientosDisponibles >= SelectedPassengers);

                var resultados = viajesFiltrados
                    .OrderBy(v => v.FechaHoraSalida)
                    .ToList();

                ViajesDisponibles.Clear();
                foreach (var viaje in resultados)
                {
                    ViajesDisponibles.Add(viaje);
                }

                if (!resultados.Any())
                {
                    await Application.Current.MainPage.DisplayAlert("Sin resultados",
                        "No se encontraron viajes que coincidan con tu búsqueda.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error en la búsqueda: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SeleccionarRutaAsync(string ruta)
        {
            if (string.IsNullOrEmpty(ruta)) return;

            var partes = ruta.Split('→');
            if (partes.Length == 2)
            {
                Origin = partes[0].Trim();
                Destination = partes[1].Trim();
                await BuscarViajesAsync();
            }
        }

        private async Task ReservarViajeAsync(Viaje viaje)
        {
            if (viaje == null) return;

            try
            {
                var usuarioActual = await _userSession.GetCurrentUserAsync();
                if (usuarioActual == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Debes iniciar sesión para hacer una reserva.", "OK");
                    return;
                }

                if (viaje.AsientosDisponibles < SelectedPassengers)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "No hay suficientes asientos disponibles.", "OK");
                    return;
                }

                var metodoPago = await Application.Current.MainPage.DisplayActionSheet(
                    "Selecciona método de pago", "Cancelar", null,
                    "Efectivo", "Tarjeta", "Transferencia");

                if (metodoPago == "Cancelar" || string.IsNullOrEmpty(metodoPago))
                    return;

                var confirmar = await Application.Current.MainPage.DisplayAlert(
                    "Confirmar Reserva",
                    $"¿Confirmas la reserva?\n\n" +
                    $"Ruta: {viaje.Origen} → {viaje.Destino}\n" +
                    $"Fecha: {viaje.FechaHoraSalida:dd/MM/yyyy HH:mm}\n" +
                    $"Precio: ${viaje.Precio:F2}\n" +
                    $"Asientos: {SelectedPassengers}\n" +
                    $"Total: ${viaje.Precio * SelectedPassengers:F2}\n" +
                    $"Método de pago: {metodoPago}",
                    "Confirmar", "Cancelar");

                if (!confirmar) return;

                IsBusy = true;

                var reservaLocal = new ReservaLocal
                {
                    ReservaRemotaId = 0,
                    ViajeId = viaje.Id,
                    UsuarioId = usuarioActual.Id,
                    Estado = EstadosReserva.Pendiente,
                    MetodoPago = metodoPago,
                    FechaReserva = DateTime.Now,
                    FechaViaje = viaje.FechaHoraSalida,
                    Origen = viaje.Origen,
                    Destino = viaje.Destino,
                    Precio = viaje.Precio * SelectedPassengers,
                    NumeroAsientos = SelectedPassengers,
                    Observaciones = "Reserva creada desde la app móvil"
                };

                await _reservaDatabase.SaveReservaAsync(reservaLocal);
                viaje.AsientosDisponibles -= SelectedPassengers;

                await Application.Current.MainPage.DisplayAlert("¡Éxito!",
                    "Tu reserva ha sido creada exitosamente. Puedes verla en 'Mis Reservas'.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error al crear la reserva: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void AplicarFiltro()
        {
            // Este método podría ser ampliado según más filtros (por hora, cercanía, etc.)
            // Por ahora está vacío.
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
