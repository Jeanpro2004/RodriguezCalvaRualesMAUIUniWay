// ViewModels/SearchRideViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class SearchRideViewModel : INotifyPropertyChanged
    {
        private readonly ViajeService _viajeService;
        private readonly ReservaDatabaseService _reservaDatabase;
        private bool _isBusy;
        private string _origin;
        private string _destination;
        private DateTime _travelDate = DateTime.Today;
        private int _selectedPassengers = 1;

        public ObservableCollection<Viaje> ViajesDisponibles { get; set; }
        public ObservableCollection<string> PassengerOptions { get; set; }

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

        public ICommand LoadViajesCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ReservarCommand { get; }

        public SearchRideViewModel(ViajeService viajeService, ReservaDatabaseService reservaDatabase)
        {
            _viajeService = viajeService;
            _reservaDatabase = reservaDatabase;

            ViajesDisponibles = new ObservableCollection<Viaje>();
            PassengerOptions = new ObservableCollection<string> { "1", "2", "3", "4" };

            LoadViajesCommand = new Command(async () => await LoadViajesAsync());
            SearchCommand = new Command(async () => await SearchViajesAsync());
            ReservarCommand = new Command<Viaje>(async (viaje) => await ReservarViajeAsync(viaje));

            // Cargar viajes al inicializar
            _ = LoadViajesAsync();
        }

        public async Task LoadViajesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var viajes = await _viajeService.GetViajesAsync();

                // Filtrar solo viajes futuros con asientos disponibles
                var viajesFiltrados = viajes?.Where(v =>
                    v.FechaHoraSalida >= DateTime.Now &&
                    v.AsientosDisponibles > 0)
                    .OrderBy(v => v.FechaHoraSalida)
                    .ToList() ?? new List<Viaje>();

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

        private async Task SearchViajesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var todosLosViajes = await _viajeService.GetViajesAsync();
                var viajesFiltrados = todosLosViajes?.AsEnumerable() ?? Enumerable.Empty<Viaje>();

                // Aplicar filtros
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
                    v.FechaHoraSalida.Date == TravelDate.Date &&
                    v.AsientosDisponibles >= SelectedPassengers &&
                    v.FechaHoraSalida >= DateTime.Now);

                var resultados = viajesFiltrados.OrderBy(v => v.FechaHoraSalida).ToList();

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

        private async Task ReservarViajeAsync(Viaje viaje)
        {
            if (viaje == null) return;

            try
            {
                // Verificar asientos disponibles
                if (viaje.AsientosDisponibles < SelectedPassengers)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "No hay suficientes asientos disponibles.", "OK");
                    return;
                }

                // Seleccionar método de pago
                var metodoPago = await Application.Current.MainPage.DisplayActionSheet(
                    "Selecciona método de pago", "Cancelar", null,
                    "Efectivo", "Transferencia");

                if (metodoPago == "Cancelar" || string.IsNullOrEmpty(metodoPago))
                    return;

                var metodoEnum = metodoPago == "Efectivo" ? MetodoPago.Efectivo : MetodoPago.Transferencia;

                // Confirmar reserva
                var totalPrecio = viaje.Precio * SelectedPassengers;
                var confirmar = await Application.Current.MainPage.DisplayAlert(
                    "Confirmar Reserva",
                    $"¿Confirmas la reserva?\n\n" +
                    $"Ruta: {viaje.Origen} → {viaje.Destino}\n" +
                    $"Fecha: {viaje.FechaHoraSalida:dd/MM/yyyy HH:mm}\n" +
                    $"Asientos: {SelectedPassengers}\n" +
                    $"Precio por asiento: ${viaje.Precio:F2}\n" +
                    $"Total: ${totalPrecio:F2}\n" +
                    $"Método de pago: {metodoPago}",
                    "Confirmar", "Cancelar");

                if (!confirmar) return;

                IsBusy = true;

                // Crear reserva local
                var reservaLocal = new ReservaLocal
                {
                    ViajeId = viaje.Id,
                    PasajeroId = SessionService.CurrentUserId,
                    Estado = Estado.Pendiente,
                    MetodoPago = metodoEnum,
                    FechaReserva = DateTime.Now,
                    FechaViaje = viaje.FechaHoraSalida,
                    Origen = viaje.Origen,
                    Destino = viaje.Destino,
                    Precio = totalPrecio,
                    NumeroAsientos = SelectedPassengers,
                    Observaciones = $"Reserva creada desde la app móvil el {DateTime.Now:dd/MM/yyyy HH:mm}"
                };

                // Guardar en base de datos local
                await _reservaDatabase.SaveReservaAsync(reservaLocal);

                await Application.Current.MainPage.DisplayAlert("¡Éxito!",
                    "Tu reserva ha sido creada exitosamente. Puedes verla en 'Mis Reservas'.", "OK");

                // Opcional: navegar a la página de reservas
                await Shell.Current.GoToAsync("//reservas");

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}