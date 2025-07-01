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
        private float temperatura = 38.0f;
        private Timer tempTimer;
        private Timer parpadeoTimer;
        private Color colorOriginal;
        private bool alertaActiva = false;

        public Form1()
        {
            InitializeComponent();
            serialPort1.Open();
            serialPort1.DataReceived += SerialPort1_DataReceived;

            // Timer para simular temperatura
            tempTimer = new Timer();
            tempTimer.Interval = 2000;
            tempTimer.Tick += TempTimer_Tick;

            // Timer para parpadeo del mapa
            colorOriginal = guna2PictureBox1.BackColor;
            parpadeoTimer = new Timer();
            parpadeoTimer.Interval = 300;
            parpadeoTimer.Tick += ParpadeoTimer_Tick;

            // Mostrar temperatura inicial
            lbTemperatura.Text = $"{temperatura:F1} °C";


        }


        private void btnConectar_Click(object sender, EventArgs e)
        {
            tempTimer.Start();
            txtInformacion.AppendText("[SYSTEM] Simulación iniciada" + Environment.NewLine);
        }

        private void TempTimer_Tick(object sender, EventArgs e)
        {
            // Aumentar temperatura gradualmente
            if (temperatura < 45.0f)
            {
                temperatura += 1.0f;
            }

            // Actualizar label
            lbTemperatura.Text = $"{temperatura:F1} °C";

            // Enviar temperatura al Arduino
            serialPort1.Write(temperatura.ToString("F1"));

            // Verificar alarma
            if (temperatura > 35.0f && !alertaActiva)
            {
                alertaActiva = true;
                parpadeoTimer.Start();
                txtInformacion.AppendText($"[ALERT] Temperatura crítica: {temperatura:F1}°C" + Environment.NewLine);
            }
            else if (temperatura <= 35.0f && alertaActiva)
            {
                alertaActiva = false;
                parpadeoTimer.Stop();
                guna2PictureBox1.BackColor = colorOriginal;
                txtInformacion.AppendText($"[SYSTEM] Temperatura normalizada: {temperatura:F1}°C" + Environment.NewLine);
            }
        }

        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort1.ReadExisting(); // Lee todo lo disponible (una o varias letras)
                this.Invoke((MethodInvoker)delegate
                {
                    txtInformacion.AppendText(data); // Añade directamente al textbox
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    txtInformacion.AppendText("[ERROR] Lectura fallida: " + ex.Message + "\n");
                });
            }
        }

        private void ParpadeoTimer_Tick(object sender, EventArgs e)
        {
            guna2PictureBox1.BackColor =
                guna2PictureBox1.BackColor == Color.Red ? colorOriginal : Color.Red;
        }

    }
}
