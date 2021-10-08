using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EjemploUDP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ServidorContador servidor = new ServidorContador();

        private void Form1_Load(object sender, EventArgs e)
        {
            servidor.ValorCambio += Servidor_ValorCambio;
        }

        private void Servidor_ValorCambio()
        {
            Invoke(new Action(() => {
                lblValor.Text = servidor.Valor.ToString();
            }));
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            servidor.Iniciar();
            if (servidor.iniciado)
            {
                btnIniciar.Text = "Detener";
            }
            else
            {
                btnIniciar.Text = "Iniciar";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            servidor.Detener();
        }
    }
}
