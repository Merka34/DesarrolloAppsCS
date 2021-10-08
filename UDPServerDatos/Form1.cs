using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPServerDatos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ServidorUDP server = new ServidorUDP();

        private void Form1_Load(object sender, EventArgs e)
        {
            server.AgregadoLista1 += Server_AgregadoLista1;
            server.AgregadoLista2 += Server_AgregadoLista2;
        }

        private void Server_AgregadoLista2(string obj)
        {
            Invoke((Action)(()=>{
                listBox2.Items.Add(obj);
            }));
        }

        private void Server_AgregadoLista1(string obj)
        {
            if (this.InvokeRequired) //La llamada viene del hilo principal o el segundo?
            {//Viene del segundo
                this.Invoke(new Action<string>(Server_AgregadoLista1), obj); // Hace callback, que apunda a este mismo y pasa el mismo parametro
                    //lo ccual hace el mismo llamado pero ahora llamado del hilo principal
            }
            else
            {
                listBox1.Items.Add(obj);
            }
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            //if (server.iniciado)
            //{
                server.Iniciar();
                btnAction.Text = "Detener";
            ///
           // else
           // {
            //    server.Detener();
             //   btnAction.Text = "Iniciar";
            //}
        }
    }
}
