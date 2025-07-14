using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class VehiculoModel : INotifyPropertyChanged
    {
        private int _id;
        private string _marca = string.Empty;
        private string _modelo = string.Empty;
        private string _color = string.Empty;
        private string _placa = string.Empty;
        private int _año;
        private int _capacidad;
        private string _tipoVehiculo = string.Empty;
        private int _conductorId;
        private string _conductorNombre = string.Empty;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Marca
        {
            get => _marca;
            set => SetProperty(ref _marca, value);
        }

        public string Modelo
        {
            get => _modelo;
            set => SetProperty(ref _modelo, value);
        }

        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public string Placa
        {
            get => _placa;
            set => SetProperty(ref _placa, value);
        }

        public int Año
        {
            get => _año;
            set => SetProperty(ref _año, value);
        }

        public int Capacidad
        {
            get => _capacidad;
            set => SetProperty(ref _capacidad, value);
        }

        public string TipoVehiculo
        {
            get => _tipoVehiculo;
            set => SetProperty(ref _tipoVehiculo, value);
        }

        public int ConductorId
        {
            get => _conductorId;
            set => SetProperty(ref _conductorId, value);
        }

        public string ConductorNombre
        {
            get => _conductorNombre;
            set => SetProperty(ref _conductorNombre, value);
        }

        // Propiedades computadas
        public string VehiculoCompleto => $"{Marca} {Modelo} {Año}";
        public string PlacaFormateada => Placa?.ToUpper();
        public string CapacidadTexto => $"{Capacidad} pasajeros";
        public string DetallesCompletos => $"{VehiculoCompleto} - {PlacaFormateada} ({Color})";

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