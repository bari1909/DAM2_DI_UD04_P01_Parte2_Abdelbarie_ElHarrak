using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;
using Veterinario_MVVM.Models;
using Veterinario_MVVM.Services;
using Veterinario_MVVM.ViewModels.Base;
using Veterinario_MVVM.Views;

namespace Veterinario_MVVM.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // 1. Atributos
        private readonly MascotasService _mascotasService = new MascotasService();
        private string _nombreUsuario = string.Empty;
        private string _rolUsuario = string.Empty;
        private Mascota? _mascotaSeleccionada;

        private int _id;
        private string _nombre = string.Empty;
        private string _especie = "Perro";
        private string _raza = string.Empty;
        private int _edad;
        private double _peso;
        private string _propietario = string.Empty;
        private string _telefono = string.Empty;
        private string _motivoConsulta = string.Empty;

        private int _totalPacientes;
        private string _mensajeEstado = "Listo";
        private object _vistaActual;

        // 2. Propiedades
        // Propiedad reactiva que aloja el ViewModel / UserControl renderizado dinámicamente
        public object VistaActual
        {
            get => _vistaActual;
            set
            {
                _vistaActual = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PacientesActivo));
                OnPropertyChanged(nameof(AdmisionActiva));
            }
        }

        public bool PacientesActivo => VistaActual == this;
        public bool AdmisionActiva => VistaActual is AdmisionViewModel;

        // Propiedades del usuario
        public string NombreUsuario
        {
            get => _nombreUsuario;
            set => SetProperty(ref _nombreUsuario, value);
        }

        public string RolUsuario
        {
            get => _rolUsuario;
            set => SetProperty(ref _rolUsuario, value);
        }

        // Colección reactiva
        public ObservableCollection<Mascota> Mascotas { get; } = new ObservableCollection<Mascota>();

        public List<string> Especies { get; } = new List<string> { "Perro", "Gato", "Ave", "Exótico" };

        // Elemento seleccionado: al cambiar se cargan los datos en el formulario lateral
        public Mascota? MascotaSeleccionada
        {
            get => _mascotaSeleccionada;
            set
            {
                if (SetProperty(ref _mascotaSeleccionada, value))
                {
                    CargarCamposSeleccionados();
                }
            }
        }

        // Propiedades del formulario lateral
        public int Id
        {
            get => _id;
            set
            {
                if (SetProperty(ref _id, value))
                {
                    OnPropertyChanged(nameof(TituloFormulario));
                }
            }
        }

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public string Especie
        {
            get => _especie;
            set => SetProperty(ref _especie, value);
        }

        public string Raza
        {
            get => _raza;
            set => SetProperty(ref _raza, value);
        }

        public int Edad
        {
            get => _edad;
            set => SetProperty(ref _edad, value);
        }

        public double Peso
        {
            get => _peso;
            set => SetProperty(ref _peso, value);
        }

        public string Propietario
        {
            get => _propietario;
            set => SetProperty(ref _propietario, value);
        }

        public string Telefono
        {
            get => _telefono;
            set => SetProperty(ref _telefono, value);
        }

        public string MotivoConsulta
        {
            get => _motivoConsulta;
            set => SetProperty(ref _motivoConsulta, value);
        }

        public string TituloFormulario
        {
            get
            {
                if (Id == 0)
                {
                    return "Alta de paciente";
                }
                return "Edición del paciente nº " + Id;
            }
        }

        // Barra inferior: contador de pacientes y mensaje de estado
        public int TotalPacientes
        {
            get => _totalPacientes;
            set => SetProperty(ref _totalPacientes, value);
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => SetProperty(ref _mensajeEstado, value);
        }

        // 3. Comandos
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand CerrarSesionCommand { get; }
        public ICommand MostrarPacientesCommand { get; }
        public ICommand MostrarAdmisionCommand { get; }

        // 4. Constructor
        public MainViewModel(Usuario usuario)
        {
            NombreUsuario = usuario.NombreCompleto;
            RolUsuario = usuario.Rol;

            // 1. Asigna la vista/contenido inicial predeterminado (Gestión de Pacientes de la Parte 1)
            _vistaActual = this;

            // 2. Comandos reactivos para conmutar el UserControl activo
            MostrarAdmisionCommand = new RelayCommand(_ => VistaActual = new AdmisionViewModel());
            MostrarPacientesCommand = new RelayCommand(_ => VistaActual = this);

            GuardarCommand = new RelayCommand(p => PerformGuardar(), p => CanExecuteGuardar());
            EliminarCommand = new RelayCommand(p => PerformEliminar(), p => MascotaSeleccionada != null);
            LimpiarCommand = new RelayCommand(p => PerformLimpiar());
            CerrarSesionCommand = new RelayCommand(p => PerformCerrarSesion(p));

            PerformCargarMascotas();
            MensajeEstado = "Bienvenido/a, " + NombreUsuario + ". Hay " + TotalPacientes + " pacientes registrados.";
        }

        // 5. Métodos Execute y CanExecute
        private void PerformCargarMascotas()
        {
            Mascotas.Clear();
            foreach (Mascota mascota in _mascotasService.ObtenerTodos())
            {
                Mascotas.Add(mascota);
            }
            TotalPacientes = Mascotas.Count;
        }

        // Sincronización bidireccional de selección
        private void CargarCamposSeleccionados()
        {
            if (MascotaSeleccionada == null)
            {
                LimpiarCampos();
                return;
            }

            Id = MascotaSeleccionada.Id;
            Nombre = MascotaSeleccionada.Nombre;
            Especie = MascotaSeleccionada.Especie;
            Raza = MascotaSeleccionada.Raza;
            Edad = MascotaSeleccionada.Edad;
            Peso = MascotaSeleccionada.Peso;
            Propietario = MascotaSeleccionada.Propietario;
            Telefono = MascotaSeleccionada.Telefono;
            MotivoConsulta = MascotaSeleccionada.MotivoConsulta;
        }

        private void LimpiarCampos()
        {
            Id = 0;
            Nombre = string.Empty;
            Especie = "Perro";
            Raza = string.Empty;
            Edad = 0;
            Peso = 0;
            Propietario = string.Empty;
            Telefono = string.Empty;
            MotivoConsulta = string.Empty;
        }

        // Comando Guardar: deshabilitado si el nombre de la mascota o el titular están vacíos
        private bool CanExecuteGuardar()
        {
            return !string.IsNullOrWhiteSpace(Nombre) && !string.IsNullOrWhiteSpace(Propietario);
        }

        private void PerformGuardar()
        {
            Mascota mascota = new Mascota
            {
                Id = Id,
                Nombre = Nombre.Trim(),
                Especie = Especie,
                Raza = Raza.Trim(),
                Edad = Edad,
                Peso = Peso,
                Propietario = Propietario.Trim(),
                Telefono = Telefono.Trim(),
                MotivoConsulta = MotivoConsulta.Trim()
            };

            // Comprobamos las DataAnnotations de la clase Mascota antes de guardar
            List<ValidationResult> errores = new List<ValidationResult>();
            bool esValida = Validator.TryValidateObject(mascota, new ValidationContext(mascota), errores, true);
            if (!esValida)
            {
                MensajeEstado = "No se ha guardado: " + errores[0].ErrorMessage;
                return;
            }

            if (mascota.Id == 0)
            {
                // Nueva alta: se guarda en el servicio y se inserta en la colección
                Mascota guardada = _mascotasService.Guardar(mascota);
                Mascotas.Add(guardada);
                MensajeEstado = "Paciente " + guardada.Nombre + " dado de alta correctamente.";
                PerformLimpiar();
            }
            else
            {
                // Modificación: se actualiza en el servicio y en la colección reactiva
                Mascota guardada = _mascotasService.Guardar(mascota);
                for (int i = 0; i < Mascotas.Count; i++)
                {
                    if (Mascotas[i].Id == guardada.Id)
                    {
                        Mascotas[i] = guardada;
                        break;
                    }
                }
                MensajeEstado = "Datos del paciente " + guardada.Nombre + " actualizados.";
                MascotaSeleccionada = guardada;
            }

            TotalPacientes = Mascotas.Count;
        }

        // Comando Eliminar: pide confirmación antes de la baja clínica
        private void PerformEliminar()
        {
            if (MascotaSeleccionada == null)
            {
                return;
            }

            Mascota mascota = MascotaSeleccionada;
            MessageBoxResult resultado = MessageBox.Show(
                "¿Desea dar de baja al paciente " + mascota.Nombre + " (" + mascota.Propietario + ")?",
                "Confirmar baja clínica",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                _mascotasService.Eliminar(mascota.Id);
                Mascotas.Remove(mascota);
                PerformLimpiar();
                TotalPacientes = Mascotas.Count;
                MensajeEstado = "Paciente " + mascota.Nombre + " dado de baja.";
            }
        }

        // Comando Limpiar: restablece el formulario y anula la selección
        private void PerformLimpiar()
        {
            MascotaSeleccionada = null;
            LimpiarCampos();
        }

        // Comando Cerrar Sesión: cierra el panel y reabre LoginView
        private void PerformCerrarSesion(object? parametro)
        {
            LoginView loginView = new LoginView();
            loginView.Show();

            if (parametro is Window ventana)
            {
                ventana.Close();
            }
        }
    }
}
