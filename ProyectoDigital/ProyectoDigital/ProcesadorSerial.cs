using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoDigital
{
    internal class ProcesadorSerial
    {
        private SerialPort puerto;
        private string buffer = "";
        private DatosSensor datos;
        private Action actualizarUI;
        private Action<string> mostrarConsola;
        private ManejadorTemperatura manejador;

        public ProcesadorSerial(SerialPort sp, DatosSensor datosSensor, Action actualizar, Action<string> consola, ManejadorTemperatura mt)
        {
            puerto = sp;
            datos = datosSensor;
            actualizarUI = actualizar;
            mostrarConsola = consola;
            manejador = mt;

            puerto.DataReceived += Serial_DataReceived;
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                buffer += puerto.ReadExisting();
                while (buffer.Contains("\n"))
                {
                    int index = buffer.IndexOf("\n");
                    string linea = buffer.Substring(0, index).Trim();
                    buffer = buffer.Substring(index + 1);
                    actualizarUI?.Invoke();

                    // Procesamiento en UI thread
                    Application.OpenForms[0].BeginInvoke((MethodInvoker)(() =>
                    {
                        ProcesarLinea(linea);
                    }));
                }
            }
            catch (Exception ex)
            {
                mostrarConsola($"[ERROR] Comunicación: {ex.Message}");
            }
        }

        private void ProcesarLinea(string linea)
        {
            if (linea.StartsWith("DATA:"))
            {
                string[] parametros = linea.Substring(5).Split(';');
                foreach (string param in parametros)
                {
                    if (string.IsNullOrWhiteSpace(param)) continue;

                    var partes = param.Split('=');
                    if (partes.Length != 2) continue;

                    string clave = partes[0].Trim();
                    string valor = partes[1].Trim();

                    switch (clave)
                    {
                        case "TEMP":
                            if (float.TryParse(valor, out float temp))
                            {
                                datos.Temperatura = temp;
                                manejador.VerificarAlarma(temp);
                                manejador.GuardarTemperatura(temp);
                            }
                            break;
                        case "OXI": datos.EstadoOxigeno = valor; break;
                        case "ESCUDO": datos.EstadoEscudo = valor; break;
                        case "ENERGIA": datos.EstadoNivelLuz = valor; break;
                        case "EEPROM":
                            if (valor.Length > 0) datos.UltimaLetraEEPROM = valor[0];
                            break;
                    }
                }
                actualizarUI?.Invoke();
            }
            else
            {
                mostrarConsola(linea);
            }
        }
    }
}
