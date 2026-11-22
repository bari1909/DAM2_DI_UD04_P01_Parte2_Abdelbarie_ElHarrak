using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Veterinario_MVVM.Models;
using Veterinario_MVVM.Services;
using Veterinario_MVVM.ViewModels.Base;

namespace Veterinario_MVVM.ViewModels
{
    public class AdmisionViewModel : ValidatableViewModel
    {
        // 1. Atributos
        private const string LetrasDni = "TRWAGMYFPDXBNJZSQVHLCKE";
        private readonly ReiacService _reiacService = new ReiacService();
        private int _numeroConsultaReiac;
        private bool _microchipDuplicado;

        private string _microchip = string.Empty;
        private string _nombreMascota = string.Empty;
        private string _especie = "Perro";
        private string _raza = string.Empty;
        private DateTime _fechaNacimiento = DateTime.Today.AddYears(-2);
        private double _peso = 10.0;
        private double _temperatura = 38.5;
        private string _titularDni = string.Empty;
        private string _titularNombre = string.Empty;
        private string _titularTelefono = string.Empty;
        private string _titularEmail = string.Empty;
        private bool _requiereCirugia;
        private int? _horasAyuno;
        private string? _tipoIntervencion;
        private bool _consentimientoFirmado;
        private string _observaciones = string.Empty;

        private bool _isBusy;
        private bool _isVerificandoMicrochip;
        private string _mensajeExito = string.Empty;

        // 2. Propiedades
        public List<string> Especies { get; } = new List<string> { "Perro", "Gato", "Ave", "Roedor", "Reptil" };
        public List<string> TiposIntervencion { get; } = new List<string> { "Esterilización", "Castración", "Limpieza dental", "Extirpación de tumor", "Traumatología" };

        // Validación Asíncrona de Microchip
        [Required(ErrorMessage = "El microchip es obligatorio.")]
        [RegularExpression(@"^\d{15}$", ErrorMessage = "El microchip debe tener exactamente 15 dígitos (ISO 11784/11785).")]
        public string Microchip
        {
            get => _microchip;
            set
            {
                if (SetProperty(ref _microchip, value))
                {
                    ValidateProperty(value);
                    _ = VerificarMicrochipAsync(value);
                }
            }
        }

        [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "El nombre de la mascota debe tener entre 2 y 40 caracteres.")]
        public string NombreMascota
        {
            get => _nombreMascota;
            set { if (SetProperty(ref _nombreMascota, value)) ValidateProperty(value); }
        }

        [Required(ErrorMessage = "La especie es obligatoria.")]
        public string Especie
        {
            get => _especie;
            set
            {
                if (SetProperty(ref _especie, value))
                {
                    ValidateProperty(value);
                    ValidarPeso();
                }
            }
        }

        [StringLength(50, ErrorMessage = "La raza no puede superar los 50 caracteres.")]
        public string Raza
        {
            get => _raza;
            set { if (SetProperty(ref _raza, value)) ValidateProperty(value); }
        }

        public DateTime FechaNacimiento
        {
            get => _fechaNacimiento;
            set { if (SetProperty(ref _fechaNacimiento, value)) ValidarFechaNacimiento(); }
        }

        [Range(0.05, 120.0, ErrorMessage = "El peso debe estar entre 0,05 y 120 kg.")]
        public double Peso
        {
            get => _peso;
            set { if (SetProperty(ref _peso, value)) ValidarPeso(); }
        }

        [Range(35.0, 42.5, ErrorMessage = "La temperatura corporal debe estar entre 35,0 y 42,5 °C.")]
        public double Temperatura
        {
            get => _temperatura;
            set { if (SetProperty(ref _temperatura, value)) ValidateProperty(value); }
        }

        // Validación Algorítmica del DNI/NIE
        [Required(ErrorMessage = "El DNI / NIE del titular es obligatorio.")]
        [RegularExpression(@"^([0-9]{8}|[XYZxyz][0-9]{7})[A-Za-z]$", ErrorMessage = "Formato no válido: 8 números y letra (DNI) o X/Y/Z, 7 números y letra (NIE).")]
        public string TitularDni
        {
            get => _titularDni;
            set { if (SetProperty(ref _titularDni, value)) ValidarDni(); }
        }

        [Required(ErrorMessage = "El nombre del titular es obligatorio.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "El nombre del titular debe tener entre 2 y 40 caracteres.")]
        public string TitularNombre
        {
            get => _titularNombre;
            set { if (SetProperty(ref _titularNombre, value)) ValidateProperty(value); }
        }

        [Required(ErrorMessage = "El teléfono del titular es obligatorio.")]
        [RegularExpression(@"^[6789]\d{8}$", ErrorMessage = "El teléfono debe tener 9 dígitos y empezar por 6, 7, 8 o 9.")]
        public string TitularTelefono
        {
            get => _titularTelefono;
            set { if (SetProperty(ref _titularTelefono, value)) ValidateProperty(value); }
        }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string TitularEmail
        {
            get => _titularEmail;
            set { if (SetProperty(ref _titularEmail, value)) ValidateProperty(value); }
        }

        // Protocolo Quirúrgico Condicional
        public bool RequiereCirugia
        {
            get => _requiereCirugia;
            set { if (SetProperty(ref _requiereCirugia, value)) ValidarProtocoloQuirurgico(); }
        }

        [Range(0, 72, ErrorMessage = "Las horas de ayuno deben estar entre 0 y 72.")]
        public int? HorasAyuno
        {
            get => _horasAyuno;
            set { if (SetProperty(ref _horasAyuno, value)) ValidarProtocoloQuirurgico(); }
        }

        public string? TipoIntervencion
        {
            get => _tipoIntervencion;
            set { if (SetProperty(ref _tipoIntervencion, value)) ValidarProtocoloQuirurgico(); }
        }

        public bool ConsentimientoFirmado
        {
            get => _consentimientoFirmado;
            set { if (SetProperty(ref _consentimientoFirmado, value)) ValidarProtocoloQuirurgico(); }
        }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string Observaciones
        {
            get => _observaciones;
            set { if (SetProperty(ref _observaciones, value)) ValidateProperty(value); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { if (SetProperty(ref _isBusy, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        public bool IsVerificandoMicrochip
        {
            get => _isVerificandoMicrochip;
            set { if (SetProperty(ref _isVerificandoMicrochip, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        public string MensajeExito
        {
            get => _mensajeExito;
            set
            {
                if (SetProperty(ref _mensajeExito, value))
                {
                    OnPropertyChanged(nameof(HayMensajeExito));
                }
            }
        }

        public bool HayMensajeExito => !string.IsNullOrEmpty(MensajeExito);

        // 3. Comandos
        public ICommand RegistrarAdmisionCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand CargarCasoInvalidoCommand { get; }
        public ICommand CargarCasoValidoCommand { get; }

        // 4. Constructor
        public AdmisionViewModel()
        {
            RegistrarAdmisionCommand = new RelayCommand(async p => await PerformRegistrarAdmisionAsync(), p => CanExecuteRegistrar());
            LimpiarCommand = new RelayCommand(p => PerformLimpiar());
            CargarCasoInvalidoCommand = new RelayCommand(p => PerformCargarCasoInvalido());
            CargarCasoValidoCommand = new RelayCommand(p => PerformCargarCasoValido());
        }

        // 5. Métodos de validación, Execute y CanExecute

        // 1. Validación Asíncrona de Microchip contra el REIAC
        private async Task VerificarMicrochipAsync(string microchip)
        {
            int numeroConsulta = ++_numeroConsultaReiac;
            _microchipDuplicado = false;

            if (!Regex.IsMatch(microchip, @"^\d{15}$"))
            {
                IsVerificandoMicrochip = false;
                return;
            }

            IsVerificandoMicrochip = true;
            bool existe = await _reiacService.ExisteMicrochipAsync(microchip);

            // Si el usuario ha seguido escribiendo, ignoramos la respuesta antigua
            if (numeroConsulta != _numeroConsultaReiac)
            {
                return;
            }

            IsVerificandoMicrochip = false;
            if (existe)
            {
                _microchipDuplicado = true;
                AddError(nameof(Microchip), "Este microchip ya está registrado en el REIAC con otro animal.");
            }
        }

        // 2. Validación Algorítmica del DNI/NIE: Índice = Número (mod 23)
        private void ValidarDni()
        {
            ValidateProperty(TitularDni, nameof(TitularDni));

            string dni = TitularDni.Trim().ToUpper();
            if (!Regex.IsMatch(dni, @"^([0-9]{8}|[XYZ][0-9]{7})[A-Z]$"))
            {
                return;
            }

            // En el NIE la letra inicial se sustituye por un número: X = 0, Y = 1, Z = 2
            string numeros = dni.Substring(0, 8);
            numeros = numeros.Replace("X", "0").Replace("Y", "1").Replace("Z", "2");

            int numero = int.Parse(numeros);
            char letraCorrecta = LetrasDni[numero % 23];
            char letraEscrita = dni[8];

            if (letraEscrita != letraCorrecta)
            {
                AddError(nameof(TitularDni), "La letra no es correcta: para este número corresponde la letra " + letraCorrecta + ".");
            }
        }

        // 3. Reglas Cruzadas (Especie vs. Peso)
        private void ValidarPeso()
        {
            ValidateProperty(Peso, nameof(Peso));

            if (Especie == "Ave" && Peso > 5.0)
            {
                AddError(nameof(Peso), "Incoherencia biométrica: un ave no puede pesar más de 5 kg.");
            }
            else if (Especie == "Roedor" && Peso > 3.0)
            {
                AddError(nameof(Peso), "Peso excesivo para un roedor: el máximo es 3 kg.");
            }
            else if (Especie == "Reptil" && Peso > 25.0)
            {
                AddError(nameof(Peso), "Peso excesivo para un reptil: el máximo es 25 kg.");
            }
        }

        // Límites de Fechas: ni fechas futuras ni más de 30 años de antigüedad
        private void ValidarFechaNacimiento()
        {
            ClearErrors(nameof(FechaNacimiento));

            if (FechaNacimiento.Date > DateTime.Today)
            {
                AddError(nameof(FechaNacimiento), "La fecha de nacimiento no puede ser futura.");
            }
            else if (FechaNacimiento.Date < DateTime.Today.AddYears(-30))
            {
                AddError(nameof(FechaNacimiento), "La fecha de nacimiento no puede tener más de 30 años de antigüedad.");
            }
        }

        // 4. Protocolo Quirúrgico Condicional
        private void ValidarProtocoloQuirurgico()
        {
            ValidateProperty(HorasAyuno, nameof(HorasAyuno));
            ClearErrors(nameof(TipoIntervencion));
            ClearErrors(nameof(ConsentimientoFirmado));

            if (!RequiereCirugia)
            {
                return;
            }

            if (HorasAyuno == null || HorasAyuno < 8)
            {
                AddError(nameof(HorasAyuno), "Para la cirugía se necesitan al menos 8 horas de ayuno.");
            }

            if (string.IsNullOrWhiteSpace(TipoIntervencion))
            {
                AddError(nameof(TipoIntervencion), "Indica el tipo de intervención quirúrgica.");
            }

            if (!ConsentimientoFirmado)
            {
                AddError(nameof(ConsentimientoFirmado), "El titular debe firmar el consentimiento informado.");
            }
        }

        // Valida todo el formulario: DataAnnotations + reglas propias
        private bool ValidarFormularioCompleto()
        {
            ValidateAll();
            ValidarDni();
            ValidarPeso();
            ValidarFechaNacimiento();
            ValidarProtocoloQuirurgico();

            if (_microchipDuplicado)
            {
                AddError(nameof(Microchip), "Este microchip ya está registrado en el REIAC con otro animal.");
            }

            return !HasErrors;
        }

        // 5. RegistrarAdmisionCommand: activo si !HasErrors && !IsBusy && !IsVerificandoMicrochip y campos obligatorios
        private bool CanExecuteRegistrar()
        {
            return !HasErrors && !IsBusy && !IsVerificandoMicrochip
                && !string.IsNullOrWhiteSpace(Microchip)
                && !string.IsNullOrWhiteSpace(NombreMascota)
                && !string.IsNullOrWhiteSpace(TitularDni)
                && !string.IsNullOrWhiteSpace(TitularNombre)
                && !string.IsNullOrWhiteSpace(TitularTelefono)
                && !string.IsNullOrWhiteSpace(TitularEmail);
        }

        private async Task PerformRegistrarAdmisionAsync()
        {
            if (!ValidarFormularioCompleto())
            {
                return;
            }

            IsBusy = true;
            try
            {
                AdmisionPaciente admision = new AdmisionPaciente
                {
                    Microchip = Microchip,
                    NombreMascota = NombreMascota.Trim(),
                    Especie = Especie,
                    Raza = Raza.Trim(),
                    FechaNacimiento = FechaNacimiento,
                    Peso = Peso,
                    Temperatura = Temperatura,
                    TitularDni = TitularDni.Trim().ToUpper(),
                    TitularNombre = TitularNombre.Trim(),
                    TitularTelefono = TitularTelefono,
                    TitularEmail = TitularEmail.Trim(),
                    RequiereCirugia = RequiereCirugia,
                    HorasAyuno = HorasAyuno,
                    TipoIntervencion = TipoIntervencion,
                    ConsentimientoFirmado = ConsentimientoFirmado,
                    Observaciones = Observaciones.Trim(),
                    FechaAdmision = DateTime.Now
                };

                // Simulamos el guardado de la admisión
                await Task.Delay(500);

                PerformLimpiar();
                MensajeExito = "Admisión de " + admision.NombreMascota + " (" + admision.Especie + ") registrada el "
                    + admision.FechaAdmision.ToString("dd/MM/yyyy 'a las' HH:mm") + ".";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // LimpiarCommand
        private void PerformLimpiar()
        {
            _numeroConsultaReiac++;
            _microchipDuplicado = false;
            IsVerificandoMicrochip = false;

            Microchip = string.Empty;
            NombreMascota = string.Empty;
            Especie = "Perro";
            Raza = string.Empty;
            FechaNacimiento = DateTime.Today.AddYears(-2);
            Peso = 10.0;
            Temperatura = 38.5;
            TitularDni = string.Empty;
            TitularNombre = string.Empty;
            TitularTelefono = string.Empty;
            TitularEmail = string.Empty;
            RequiereCirugia = false;
            HorasAyuno = null;
            TipoIntervencion = null;
            ConsentimientoFirmado = false;
            Observaciones = string.Empty;
            MensajeExito = string.Empty;

            ClearErrors(null);
        }

        // CargarCasoInvalidoCommand: datos con fallos para probar estilos
        private void PerformCargarCasoInvalido()
        {
            PerformLimpiar();

            Microchip = "123456789012345";
            NombreMascota = "K";
            Especie = "Ave";
            Raza = "Agapornis";
            FechaNacimiento = DateTime.Today.AddDays(15);
            Peso = 7.5;
            Temperatura = 44.0;
            TitularDni = "12345678A";
            TitularNombre = string.Empty;
            TitularTelefono = "512345678";
            TitularEmail = "correo@";
            RequiereCirugia = true;
            HorasAyuno = 4;
            TipoIntervencion = null;
            ConsentimientoFirmado = false;

            ValidarFormularioCompleto();
        }

        // CargarCasoValidoCommand
        private void PerformCargarCasoValido()
        {
            PerformLimpiar();

            Microchip = "941000024587136";
            NombreMascota = "Toby";
            Especie = "Perro";
            Raza = "Labrador";
            FechaNacimiento = DateTime.Today.AddYears(-3);
            Peso = 24.5;
            Temperatura = 38.6;
            TitularDni = "12345678Z";
            TitularNombre = "Ana García López";
            TitularTelefono = "600111222";
            TitularEmail = "ana.garcia@correo.com";
            RequiereCirugia = true;
            HorasAyuno = 12;
            TipoIntervencion = "Esterilización";
            ConsentimientoFirmado = true;
            Observaciones = "Paciente tranquilo. Sin alergias conocidas.";

            ValidarFormularioCompleto();
        }
    }
}
