using System;
using System.Collections.Generic;
using Veterinario_MVVM.Models;

namespace Veterinario_MVVM.Services
{
    // MascotasService.cs: Gestiona la persistencia en memoria de los pacientes
    public class MascotasService
    {
        // Lista estática para que los datos se mantengan al cerrar sesión y volver a entrar
        private static readonly List<Mascota> _mascotas = new List<Mascota>
        {
            new Mascota { Id = 1, Nombre = "Toby", Especie = "Perro", Raza = "Labrador", Edad = 3, Peso = 24.5, Propietario = "Ana García López", Telefono = "600111222", MotivoConsulta = "Revisión anual y vacuna de la rabia.", FechaRegistro = new DateTime(2026, 9, 12, 10, 30, 0) },
            new Mascota { Id = 2, Nombre = "Misi", Especie = "Gato", Raza = "Común europeo", Edad = 5, Peso = 4.2, Propietario = "Pedro Sánchez Ruiz", Telefono = "611333444", MotivoConsulta = "Pérdida de apetito desde hace tres días.", FechaRegistro = new DateTime(2026, 10, 3, 17, 15, 0) },
            new Mascota { Id = 3, Nombre = "Kiwi", Especie = "Ave", Raza = "Agapornis", Edad = 2, Peso = 0.06, Propietario = "Marta Gómez Vidal", Telefono = "622444555", MotivoConsulta = "Corte de uñas y pico.", FechaRegistro = new DateTime(2026, 10, 20, 12, 0, 0) },
            new Mascota { Id = 4, Nombre = "Rocky", Especie = "Exótico", Raza = "Iguana verde", Edad = 6, Peso = 3.8, Propietario = "Javier Torres Molina", Telefono = "633555666", MotivoConsulta = "Control de temperatura del terrario y muda.", FechaRegistro = new DateTime(2026, 11, 5, 9, 45, 0) },
            new Mascota { Id = 5, Nombre = "Nala", Especie = "Perro", Raza = "Pastor alemán", Edad = 6, Peso = 31.0, Propietario = "Lucía Fernández Gil", Telefono = "644666777", MotivoConsulta = "Cojera en la pata trasera izquierda.", FechaRegistro = new DateTime(2026, 11, 10, 16, 20, 0) }
        };

        public List<Mascota> ObtenerTodos()
        {
            return new List<Mascota>(_mascotas);
        }

        public Mascota Guardar(Mascota m)
        {
            // Si Id == 0 es una nueva alta: le asignamos el siguiente Id libre
            if (m.Id == 0)
            {
                int siguienteId = 1;
                foreach (Mascota mascota in _mascotas)
                {
                    if (mascota.Id >= siguienteId)
                    {
                        siguienteId = mascota.Id + 1;
                    }
                }

                m.Id = siguienteId;
                m.FechaRegistro = DateTime.Now;
                _mascotas.Add(m);
                return m;
            }

            // Si Id > 0 es una modificación: sustituimos el paciente guardado manteniendo su fecha de registro
            for (int i = 0; i < _mascotas.Count; i++)
            {
                if (_mascotas[i].Id == m.Id)
                {
                    m.FechaRegistro = _mascotas[i].FechaRegistro;
                    _mascotas[i] = m;
                    return m;
                }
            }

            _mascotas.Add(m);
            return m;
        }

        public bool Eliminar(int id)
        {
            for (int i = 0; i < _mascotas.Count; i++)
            {
                if (_mascotas[i].Id == id)
                {
                    _mascotas.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
    }
}
