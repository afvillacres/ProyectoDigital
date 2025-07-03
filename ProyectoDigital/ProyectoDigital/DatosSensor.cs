using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDigital
{
    internal class DatosSensor
    {
        public float Temperatura { get; set; } = 0.0f;
        public string EstadoOxigeno { get; set; } = "NORMAL";
        public string EstadoEscudo { get; set; } = "INACTIVO";
        public string EstadoNivelLuz { get; set; } = "ALTO";
        public char UltimaLetraEEPROM { get; set; } = '?';
    }
}
