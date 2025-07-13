using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class ReservaModel : INotifyPropertyChanged
    {
        private int _id;
        private int _viajeId;
        private int _usuarioId;
        private string _usuarioNombre;
        private DateTime _fechaReserva;
        private string _estado;
        private decimal _montoTotal;
        private string _metodoPago;
        private string _notas;
        private ViajeModel _viaje;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public int ViajeId
        {
            get => _viajeId;
            set => SetProperty(ref _viajeId, value);
        }

        public int UsuarioId
        {
            get => _usuarioId;
            set => SetProperty(ref _usuarioId, value);
        }

        public string UsuarioNombre
        {
            get => _usuarioNombre;
            set => SetProperty(ref _usuarioNombre, value);
        }

        public DateTime FechaReserva
        {
            get => _fechaReserva;
            set => SetProperty(ref _fechaReserva, value);
        }

        public string Estado
        {
            get => _estado;
            set => SetProperty(ref _estado, value);
        }

        public decimal MontoTotal
        {
            get => _montoTotal;
            set => SetProperty(ref _montoTotal, value);
        }

        public string MetodoPago
        {
            get => _metodoPago;
            set => SetProperty(ref _metodoPago, value);
        }

        public string Notas
        {
            get => _notas;
            set => SetProperty(ref _notas, value);
        }

        public ViajeModel Viaje
        {
            get => _viaje;
            set => SetProperty(ref _viaje, value);
        }

        // Propiedades computadas
        public string FechaReservaFormateada => FechaReserva.ToString("dd/MM/yyyy HH:mm");
        public string MontoFormateado => $"${MontoTotal:F2}";
        public string EstadoColor => Estado switch
        {
            "Confirmada" => "#27AE60",
            "Pendiente" => "#F39C12",
            "Cancelada" => "#E74C3C",
            _ => "#3498DB"
        };

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
