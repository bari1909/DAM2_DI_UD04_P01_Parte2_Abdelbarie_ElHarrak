using System;
using System.ComponentModel.DataAnnotations;

namespace Veterinario_MVVM.Models
{
    public class AdmisionPaciente
    {
        [Key]
        public int Id { get; set; }

        // Microchip Oficial (15 dígitos): expresión regular ^\d{15}$ conforme a ISO 11784/11785
        [Required(ErrorMessage = "El microchip es obligatorio.")]
        [RegularExpression(@"^\d{15}$", ErrorMessage = "El microchip debe tener exactamente 15 dígitos (ISO 11784/11785).")]
        [Display(Name = "Microchip")]
        public string Microchip { get; set; } = string.Empty;

        // Obligatoriedad y Longitud: [Required], [StringLength(40, MinimumLength = 2)]
        [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "El nombre de la mascota debe tener entre 2 y 40 caracteres.")]
        [Display(Name = "Nombre de la Mascota")]
        public string NombreMascota { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especie es obligatoria.")]
        [Display(Name = "Especie")]
        public string Especie { get; set; } = "Perro";

        [StringLength(50, ErrorMessage = "La raza no puede superar los 50 caracteres.")]
        [Display(Name = "Raza")]
        public string Raza { get; set; } = string.Empty;

        // Límites de Fechas: no se admiten fechas futuras ni con más de 30 años (se comprueba en el ViewModel)
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; } = DateTime.Today.AddYears(-2);

        // Rangos Biométricos Fisiológicos: [Range(0.05, 120.0)]
        [Range(0.05, 120.0, ErrorMessage = "El peso debe estar entre 0,05 y 120 kg.")]
        [Display(Name = "Peso (kg)")]
        public double Peso { get; set; } = 10.0;

        // Rangos Biométricos Fisiológicos: [Range(35.0, 42.5)] (°C corporal)
        [Range(35.0, 42.5, ErrorMessage = "La temperatura corporal debe estar entre 35,0 y 42,5 °C.")]
        [Display(Name = "Temperatura (°C)")]
        public double Temperatura { get; set; } = 38.5;

        // Formato Regex e Identidad: DNI / NIE (la letra se comprueba con el módulo 23 en el ViewModel)
        [Required(ErrorMessage = "El DNI / NIE del titular es obligatorio.")]
        [RegularExpression(@"^([0-9]{8}|[XYZxyz][0-9]{7})[A-Za-z]$", ErrorMessage = "Formato no válido: 8 números y letra (DNI) o X/Y/Z, 7 números y letra (NIE).")]
        [Display(Name = "DNI / NIE del Titular")]
        public string TitularDni { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del titular es obligatorio.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "El nombre del titular debe tener entre 2 y 40 caracteres.")]
        [Display(Name = "Nombre del Titular")]
        public string TitularNombre { get; set; } = string.Empty;

        // Canales de Contacto: Regex ^[6789]\d{8}$ (móvil nacional de 9 dígitos)
        [Required(ErrorMessage = "El teléfono del titular es obligatorio.")]
        [RegularExpression(@"^[6789]\d{8}$", ErrorMessage = "El teléfono debe tener 9 dígitos y empezar por 6, 7, 8 o 9.")]
        [Display(Name = "Teléfono")]
        public string TitularTelefono { get; set; } = string.Empty;

        // Canales de Contacto: [EmailAddress]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo Electrónico")]
        public string TitularEmail { get; set; } = string.Empty;

        // Protocolo Quirúrgico Condicional: si RequiereCirugia == true exige ayuno, intervención y firma (ViewModel)
        [Display(Name = "Requiere Cirugía")]
        public bool RequiereCirugia { get; set; } = false;

        [Range(0, 72, ErrorMessage = "Las horas de ayuno deben estar entre 0 y 72.")]
        [Display(Name = "Horas de Ayuno")]
        public int? HorasAyuno { get; set; }

        [StringLength(60, ErrorMessage = "El tipo de intervención no puede superar los 60 caracteres.")]
        [Display(Name = "Tipo de Intervención")]
        public string? TipoIntervencion { get; set; }

        [Display(Name = "Consentimiento Firmado")]
        public bool ConsentimientoFirmado { get; set; } = false;

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Admisión")]
        public DateTime FechaAdmision { get; set; } = DateTime.Now;
    }
}
