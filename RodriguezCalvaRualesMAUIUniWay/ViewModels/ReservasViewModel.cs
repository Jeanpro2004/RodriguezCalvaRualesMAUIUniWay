using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class ReservasViewModel : INotifyPropertyChanged
    {
        private readonly ReservaDatabaseLocal _database;
        private bool _isBusy;
        private string _filtroEstado = "Todas";

        public ObservableCollection<ReservaLocal> Reservas { get; set; }
        public ObservableCollection<string> EstadosFiltro { get; set; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                SetProperty(ref _filtroEstado, value);
                _ = LoadReservasAsync();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }

        public ReservasViewModel(ReservaDatabaseLocal database)
        {
            _database = database;
            Reservas = new ObservableCollection<ReservaLocal>();
            EstadosFiltro = new ObservableCollection<string>
            {
                "Todas", "Pendiente", "Confirmada", "Cancelada", "Completada"
            };

            RefreshCommand = new Command(async () => await LoadReservasAsync());
            DeleteCommand = new Command<ReservaLocal>(async (reserva) => await DeleteReservaAsync(reserva));
            EditCommand = new Command<ReservaLocal>(async (reserva) => await EditReservaAsync(reserva));
        }

        public async Task LoadReservasAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var reservas = FiltroEstado == "Todas"
                    ? await _database.GetReservasAsync()
                    : await _database.GetReservasByEstadoAsync(FiltroEstado);

                Reservas.Clear();
                foreach (var reserva in reservas)
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
                "Confirmar",
                $"¿Estás seguro de eliminar la reserva para {reserva.RutaCompleta}?",
                "Sí", "No");

            if (confirm)
            {
                try
                {
                    await _database.DeleteReservaAsync(reserva);
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

        private async Task EditReservaAsync(ReservaLocal reserva)
        {
            // Navegar a página de edición
            // await Shell.Current.GoToAsync($"editreserva?id={reserva.Id}");
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