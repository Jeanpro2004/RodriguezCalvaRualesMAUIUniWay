using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class ViajeModel : INotifyPropertyChanged
    {
        private int _id;
        private string _origen;
        private string _destino;
        private DateTime _fecha;
        private TimeSpan _hora;
        private int _espaciosDisponibles;
        private int _espaciosTotales;
        private decimal _precio;
        private string _conductor;
        private int _conductorId;
        private string _estado;
        private string _descripcion;
        private List<string> _pasajeros;
        private VehiculoModel _vehiculo;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Origen
        {
            get => _origen;
            set => SetProperty(ref _origen, value);
        }

        public string Destino
        {
            get => _destino;
            set => SetProperty(ref _destino, value);
        }

        public DateTime Fecha
        {
            get => _fecha;
            set => SetProperty(ref _fecha, value);
        }

        public TimeSpan Hora
        {
            get => _hora;
            set => SetProperty(ref _hora, value);
        }

        public int EspaciosDisponibles
        {
            get => _espaciosDisponibles;
            set => SetProperty(ref _espaciosDisponibles, value);
        }

        public int EspaciosTotales
        {
            get => _espaciosTotales;
            set => SetProperty(ref _espaciosTotales, value);
        }

        public decimal Precio
        {
            get => _precio;
            set => SetProperty(ref _precio, value);
        }

        public string Conductor
        {
            get => _conductor;
            set => SetProperty(ref _conductor, value);
        }

        public int ConductorId
        {
            get => _conductorId;
            set => SetProperty(ref _conductorId, value);
        }

        public string Estado
        {
            get => _estado;
            set => SetProperty(ref _estado, value);
        }

        public string Descripcion
        {
            get => _descripcion;
            set => SetProperty(ref _descripcion, value);
        }

        public List<string> Pasajeros
        {
            get => _pasajeros ??= new List<string>();
            set => SetProperty(ref _pasajeros, value);
        }

        public VehiculoModel Vehiculo
        {
            get => _vehiculo;
            set => SetProperty(ref _vehiculo, value);
        }

        // Propiedades computadas
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");
        public string HoraFormateada => Hora.ToString(@"hh\:mm");
        public string FechaHoraCompleta => $"{FechaFormateada} a las {HoraFormateada}";
        public string RutaCompleta => $"{Origen} → {Destino}";
        public string PrecioFormateado => $"${Precio:F2}";
        public string EspaciosInfo => $"{EspaciosDisponibles}/{EspaciosTotales} disponibles";
        public bool TieneEspacios => EspaciosDisponibles > 0;
        public bool EstaCompleto => EspaciosDisponibles == 0;
        public string EstadoColor => Estado switch
        {
            "Activo" => "#27AE60",
            "Completo" => "#E74C3C",
            "Cancelado" => "#95A5A6",
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
