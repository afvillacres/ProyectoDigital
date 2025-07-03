using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoDigital
{
    public partial class Form1 : Form
    {
        private DatosSensor datosSensor;
        private ManejadorTemperatura manejador;
        private ProcesadorSerial procesador;

        public Form1()
        {
            InitializeComponent();
            datosSensor = new DatosSensor();
            manejador = new ManejadorTemperatura(guna2PictureBox1, txtInformacion);
            procesador = new ProcesadorSerial(serialPort1, datosSensor, ActualizarInterfaz, MostrarConsola, manejador);

            serialPort1.BaudRate = 9600;
            serialPort1.DataBits = 8;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;


        }

        private void ActualizarInterfaz()
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)ActualizarInterfaz);
                return;
            }

            lbTemperatura.Text = $"{datosSensor.Temperatura:F1} °C";
            lbOxigeno.Text = datosSensor.EstadoOxigeno;
            lbEscudo.Text = datosSensor.EstadoEscudo;
            lbNivelLuz.Text = datosSensor.EstadoNivelLuz;
            lbComunicacion.Text = datosSensor.UltimaLetraEEPROM != '?' ? $"EEPROM: {datosSensor.UltimaLetraEEPROM}" : "";

            lbOxigeno.ForeColor = datosSensor.EstadoOxigeno == "BAJO" ? Color.Red : Color.Green;
            lbEscudo.ForeColor = datosSensor.EstadoEscudo == "ACTIVO" ? Color.Green : Color.Gray;
            lbNivelLuz.ForeColor = datosSensor.EstadoNivelLuz == "BAJO" ? Color.Orange : Color.Yellow;
        }


        private void MostrarConsola(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => MostrarConsola(msg)));
                return;
            }

            txtInformacion.AppendText(msg + Environment.NewLine);
            txtInformacion.SelectionStart = txtInformacion.Text.Length;
            txtInformacion.ScrollToCaret();
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    MostrarConsola("[SYSTEM] Puerto serie conectado");
                }
                else
                {
                    MostrarConsola("[SYSTEM] Puerto ya conectado");
                }
            }
            catch (Exception ex)
            {
                MostrarConsola($"[ERROR] {ex.Message}");
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                MostrarConsola("[SYSTEM] Solicitando datos...");
            else
                MostrarConsola("[ERROR] Puerto no conectado");
        }

        private void btnMostrarGrafica_Click(object sender, EventArgs e)
        {
            manejador.MostrarGrafica(chart1);
        }
    }
}
