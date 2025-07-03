using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guna.UI2.WinForms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.IO;

namespace ProyectoDigital
{
    internal class ManejadorTemperatura
    {
        private Timer parpadeoTimer;
        private Color colorOriginal;
        private Control mapa;
        private Guna2TextBox consola;
        private bool alertaActiva = false;

        public ManejadorTemperatura(Control mapaControl, Guna2TextBox salidaConsola)
        {
            mapa = mapaControl;
            consola = salidaConsola;
            colorOriginal = mapa.BackColor;

            parpadeoTimer = new Timer();
            parpadeoTimer.Interval = 300;
            parpadeoTimer.Tick += (s, e) => mapa.BackColor =
                (mapa.BackColor == Color.Red) ? colorOriginal : Color.Red;
        }

        public void VerificarAlarma(float temperatura)
        {
            if (temperatura > 35.0f && !alertaActiva)
            {
                alertaActiva = true;
                parpadeoTimer.Start();
                consola.AppendText($"[ALERT] Temperatura crítica: {temperatura:F1}°C\n");
            }
            else if (temperatura <= 35.0f && alertaActiva)
            {
                alertaActiva = false;
                parpadeoTimer.Stop();
                mapa.BackColor = colorOriginal;
                consola.AppendText($"[SYSTEM] Temperatura normalizada: {temperatura:F1}°C\n");
            }
        }

        public void GuardarTemperatura(float temp)
        {
            string ruta = "temperaturas.csv";
            string linea = $"{DateTime.Now},{temp:F1}";
            File.AppendAllText(ruta, linea + Environment.NewLine);

        }

        public void MostrarGrafica(Chart chart)
        {
            string ruta = "temperaturas.csv";
            if (!File.Exists(ruta))
            {
                MessageBox.Show("No hay datos registrados aún.");
                return;
            }

            chart.Series.Clear();
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chart.ChartAreas[0].AxisX.Interval = 1;

            Series serie = new Series("Temperatura") { ChartType = SeriesChartType.Line };
            chart.Series.Add(serie);

            foreach (string linea in File.ReadAllLines(ruta))
            {
                var partes = linea.Split(',');
                if (partes.Length == 2 &&
                    DateTime.TryParse(partes[0], out DateTime tiempo) &&
                    float.TryParse(partes[1], out float temp))
                {
                    serie.Points.AddXY(tiempo.ToString("HH:mm:ss"), temp);
                }
            }
        }
    }
}
