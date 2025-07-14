// ViewModels/ReservasViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RodriguezCalvaRualesMAUIUniWay.API;
using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class ReservasViewModel : INotifyPropertyChanged
    {
        private readonly ReservaDatabaseService _reservaDatabase;
        private bool _isBusy;
        private Estado? _filtroEstado;

        public ObservableCollection<ReservaLocal> Reservas { get; set; }
        public ObservableCollection<string> EstadosFiltro { get; set; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public Estado? FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                SetProperty(ref _filtroEstado, value);
                _ = LoadReservasAsync();
            }
        }

        public ICommand LoadReservasCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand DeleteReservaCommand { get; }
        public ICommand CancelReservaCommand { get; }

        public ReservasViewModel(ReservaDatabaseService reservaDatabase)
        {
            _reservaDatabase = reservaDatabase;

            Reservas = new ObservableCollection<ReservaLocal>();
            EstadosFiltro = new ObservableCollection<string>
            {
                "Todas", "Pendiente", "Confirmada", "Cancelada"
            };

            LoadReservasCommand = new Command(async () => await LoadReservasAsync());
            RefreshCommand = new Command(async () => await LoadReservasAsync());
            DeleteReservaCommand = new Command<ReservaLocal>(async (reserva) => await DeleteReservaAsync(reserva));
            CancelReservaCommand = new Command<ReservaLocal>(async (reserva) => await CancelReservaAsync(reserva));

            // Cargar reservas al inicializar
            _ = LoadReservasAsync();
        }

        public async Task LoadReservasAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                List<ReservaLocal> reservas;

                if (FiltroEstado.HasValue)
                {
                    reservas = await _reservaDatabase.GetReservasByEstadoAsync(FiltroEstado.Value);
                }
                else
                {
                    reservas = await _reservaDatabase.GetReservasAsync();
                }

                // Filtrar solo las reservas del usuario actual
                var reservasUsuario = reservas.Where(r => r.PasajeroId == SessionService.CurrentUserId).ToList();

                Reservas.Clear();
                foreach (var reserva in reservasUsuario)
                {
                    Reservas.Add(reserva);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error al cargar reservas: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteReservaAsync(ReservaLocal reserva)
        {
            if (reserva == null) return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirmar eliminación",
                $"¿Estás seguro de eliminar la reserva para {reserva.RutaCompleta}?",
                "Sí", "No");

            if (confirm)
            {
                try
                {
                    await _reservaDatabase.DeleteReservaAsync(reserva);
                    Reservas.Remove(reserva);
                    await Application.Current.MainPage.DisplayAlert("Éxito",
                        "Reserva eliminada correctamente", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        $"Error al eliminar reserva: {ex.Message}", "OK");
                }
            }
        }

        private async Task CancelReservaAsync(ReservaLocal reserva)
        {
            if (reserva == null || reserva.Estado == Estado.Cancelada) return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Cancelar reserva",
                $"¿Estás seguro de cancelar la reserva para {reserva.RutaCompleta}?",
                "Sí", "No");

            if (confirm)
            {
                try
                {
                    reserva.Estado = Estado.Cancelada;
                    await _reservaDatabase.SaveReservaAsync(reserva);

                    // Actualizar la UI
                    var index = Reservas.IndexOf(reserva);
                    if (index >= 0)
                    {
                        Reservas[index] = reserva;
                    }

                    await Application.Current.MainPage.DisplayAlert("Éxito",
                        "Reserva cancelada correctamente", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        $"Error al cancelar reserva: {ex.Message}", "OK");
                }
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