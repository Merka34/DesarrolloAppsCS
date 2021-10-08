using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorHTTPSimple
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            servidor.NombreAgregado += Servidor_NombreAgregado;
            servidor.Iniciar();
        }

        private void Servidor_NombreAgregado(string obj)
        {
            this.Invoke((MethodInvoker)(()=> {
                listBox1.Items.Add(obj);
            }));
        }

        ServidorHTTP servidor = new ServidorHTTP();
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
