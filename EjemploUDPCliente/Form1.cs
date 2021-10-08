using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EjemploUDPCliente
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ClienteUDP cliente = new ClienteUDP();

        private void btnIncrementar_Click(object sender, EventArgs e)
        {
            IPAddress ip;

            if (IPAddress.TryParse(txtIP.Text, out ip))
            {
                cliente.Enviar(ip);
            }
            else
            {
                MessageBox.Show("No es una direccion IP válida");
            }
        }
    }
}
