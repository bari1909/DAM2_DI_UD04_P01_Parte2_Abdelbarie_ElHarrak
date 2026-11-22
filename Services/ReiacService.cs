using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Veterinario_MVVM.Services
{
    public class ReiacService
    {
        private static readonly HashSet<string> _microchipsRegistrados = new(StringComparer.OrdinalIgnoreCase)
        {
            "999000000000001", "999000000000002", "111222333444555", "123456789012345"
        };

        public async Task<bool> ExisteMicrochipAsync(string microchip)
        {
            if (string.IsNullOrWhiteSpace(microchip) || microchip.Length != 15) return false;
            await Task.Delay(700); // Simulación de latencia telemática
            return _microchipsRegistrados.Contains(microchip.Trim());
        }
    }
}
