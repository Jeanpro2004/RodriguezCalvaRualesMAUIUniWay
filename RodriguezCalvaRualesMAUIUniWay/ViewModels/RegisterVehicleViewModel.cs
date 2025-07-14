using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.Services;
using RodriguezCalvaRualesMAUIUniWay.Repositorios;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RodriguezCalvaRualesMAUIUniWay.ViewModels
{
    public class RegisterVehicleViewModel : BaseViewModel
    {
        private readonly VehiculoService _vehiculoService;
        private readonly ManejoArchivosRepository _archivoRepository;

        // Propiedades del formulario
        private string _marca = string.Empty;
        private string _modelo = string.Empty;
        private string _color = string.Empty;
        private string _placa = string.Empty;
        private int _año = DateTime.Now.Year;
        private int _capacidad = 4;
        private string _tipoVehiculo = "Sedan";

        public RegisterVehicleViewModel(VehiculoService vehiculoService, ManejoArchivosRepository archivoRepository)
        {
            _vehiculoService = vehiculoService;
            _archivoRepository = archivoRepository;
            Title = "Registrar Vehículo";

            // Inicializar listas
            InitializeLists();

            // Comandos
            RegisterVehicleCommand = new Command(async () => await ExecuteRegisterVehicle(), () => CanExecuteRegister());
            ClearFormCommand = new Command(() => ClearForm());
            CancelCommand = new Command(async () => await ExecuteCancel());
        }

        #region Propiedades

        public string Marca
        {
            get => _marca;
            set
            {
                SetProperty(ref _marca, value);
                ((Command)RegisterVehicleCommand).ChangeCanExecute();
            }
        }

        public string Modelo
        {
            get => _modelo;
            set
            {
                SetProperty(ref _modelo, value);
                ((Command)RegisterVehicleCommand).ChangeCanExecute();
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                SetProperty(ref _color, value);
                ((Command)RegisterVehicleCommand).ChangeCanExecute();
            }
        }

        public string Placa
        {
            get => _placa;
            set
            {
                SetProperty(ref _placa, value?.ToUpper());
                ((Command)RegisterVehicleCommand).ChangeCanExecute();
            }
        }

        public int Año
        {
            get => _año;
            set
            {
                SetProperty(ref _año, value);
                ((Command)RegisterVehicleCommand).ChangeCanExecute();
            }
        }

        public int Capacidad
        {
            get => _capacidad;
            set
            {
                SetProperty(ref _capacidad, value);
                ((Command)RegisterVehicleCommand).ChangeCanExecute();
            }
        }

        public string TipoVehiculo
        {
            get => _tipoVehiculo;
            set => SetProperty(ref _tipoVehiculo, value);
        }

        // Listas para Pickers
        public ObservableCollection<string> MarcasDisponibles { get; set; }
        public ObservableCollection<string> ColoresDisponibles { get; set; }
        public ObservableCollection<string> TiposVehiculo { get; set; }
        public ObservableCollection<int> AñosDisponibles { get; set; }
        public ObservableCollection<int> CapacidadesDisponibles { get; set; }

        #endregion

        #region Comandos

        public ICommand RegisterVehicleCommand { get; }
        public ICommand ClearFormCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        #region Métodos Privados

        private void InitializeLists()
        {
            MarcasDisponibles = new ObservableCollection<string>
            {
                "Toyota", "Chevrolet", "Nissan", "Hyundai", "Kia", "Ford", "Volkswagen",
                "Mazda", "Honda", "Mitsubishi", "Suzuki", "Renault", "Peugeot", "Otro"
            };

            ColoresDisponibles = new ObservableCollection<string>
            {
                "Blanco", "Negro", "Gris", "Plata", "Azul", "Rojo", "Verde", "Amarillo",
                "Café", "Dorado", "Naranja", "Morado", "Rosa", "Otro"
            };

            TiposVehiculo = new ObservableCollection<string>
            {
                "Sedan", "SUV", "Hatchback", "Camioneta", "Pick-up", "Coupe", "Convertible", "Otro"
            };

            AñosDisponibles = new ObservableCollection<int>();
            for (int i = DateTime.Now.Year; i >= 1990; i--)
            {
                AñosDisponibles.Add(i);
            }

            CapacidadesDisponibles = new ObservableCollection<int> { 2, 3, 4, 5, 6, 7, 8 };
        }

        private bool CanExecuteRegister()
        {
            return !string.IsNullOrWhiteSpace(Marca) &&
                   !string.IsNullOrWhiteSpace(Modelo) &&
                   !string.IsNullOrWhiteSpace(Color) &&
                   !string.IsNullOrWhiteSpace(Placa) &&
                   Año >= 1990 && Año <= DateTime.Now.Year &&
                   Capacidad >= 2 && Capacidad <= 8 &&
                   !IsBusy;
        }

        private async Task ExecuteRegisterVehicle()
        {
            try
            {
                IsBusy = true;

                // Validar placa ecuatoriana
                if (!ValidarPlacaEcuatoriana(Placa))
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "La placa debe tener el formato ecuatoriano (ej: ABC-1234)", "OK");
                    return;
                }

                // Obtener usuario actual
                var usuarioActual = await _archivoRepository.ObtenerUsuarioActual();
                if (usuarioActual == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "No hay usuario logueado", "OK");
                    return;
                }

                // Verificar que el usuario sea conductor
                if (!usuarioActual.EsConductor)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Solo los conductores pueden registrar vehículos", "OK");
                    return;
                }

                // Verificar si ya existe un vehículo con esa placa
                var vehiculosExistentes = await _vehiculoService.GetVehiculosLocalesAsync();
                if (vehiculosExistentes.Any(v => v.Placa.Equals(Placa, StringComparison.OrdinalIgnoreCase)))
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Ya existe un vehículo registrado con esa placa", "OK");
                    return;
                }

                // Crear el vehículo
                var vehiculo = new VehiculoModel
                {
                    Marca = Marca.Trim(),
                    Modelo = Modelo.Trim(),
                    Color = Color.Trim(),
                    Placa = Placa.Trim().ToUpper(),
                    Año = Año,
                    Capacidad = Capacidad,
                    TipoVehiculo = TipoVehiculo,
                    ConductorId = usuarioActual.Id,
                    ConductorNombre = usuarioActual.Nombre
                };

                // Guardar localmente
                var success = await _vehiculoService.GuardarVehiculoLocalAsync(vehiculo);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("¡Éxito!",
                        $"Vehículo {vehiculo.VehiculoCompleto} registrado correctamente", "OK");

                    // Limpiar formulario
                    ClearForm();

                    // Opcional: Navegar de regreso
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "No se pudo registrar el vehículo", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registrando vehículo: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Ocurrió un error: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidarPlacaEcuatoriana(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                return false;

            // Formato ecuatoriano: ABC-1234 o AB-1234
            var patron = @"^[A-Z]{2,3}-\d{3,4}$";
            return System.Text.RegularExpressions.Regex.IsMatch(placa.ToUpper(), patron);
        }

        private void ClearForm()
        {
            Marca = string.Empty;
            Modelo = string.Empty;
            Color = string.Empty;
            Placa = string.Empty;
            Año = DateTime.Now.Year;
            Capacidad = 4;
            TipoVehiculo = "Sedan";
        }

        private async Task ExecuteCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        #endregion
    }
}