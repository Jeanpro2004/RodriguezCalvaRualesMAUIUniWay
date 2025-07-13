using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class NotificacionModel : INotifyPropertyChanged
    {
        private int _id;
        private string _titulo;
        private string _mensaje;
        private DateTime _fecha;
        private bool _leida;
        private string _tipo;
        private int _usuarioId;
        private string _icono;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Titulo
        {
            get => _titulo;
            set => SetProperty(ref _titulo, value);
        }

        public string Mensaje
        {
            get => _mensaje;
            set => SetProperty(ref _mensaje, value);
        }

        public DateTime Fecha
        {
            get => _fecha;
            set => SetProperty(ref _fecha, value);
        }

        public bool Leida
        {
            get => _leida;
            set => SetProperty(ref _leida, value);
        }

        public string Tipo
        {
            get => _tipo;
            set => SetProperty(ref _tipo, value);
        }

        public int UsuarioId
        {
            get => _usuarioId;
            set => SetProperty(ref _usuarioId, value);
        }

        public string Icono
        {
            get => _icono;
            set => SetProperty(ref _icono, value);
        }

        // Propiedades computadas
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy HH:mm");
        public string TiempoTranscurrido
        {
            get
            {
                var diff = DateTime.Now - Fecha;
                if (diff.TotalMinutes < 1) return "Ahora";
                if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes} min";
                if (diff.TotalDays < 1) return $"{(int)diff.TotalHours} h";
                return $"{(int)diff.TotalDays} días";
            }
        }

        public string BackgroundColor => Leida ? "#F8F9FA" : "#E3F2FD";
        public string TextColor => Leida ? "#95A5A6" : "#2C3E50";

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
