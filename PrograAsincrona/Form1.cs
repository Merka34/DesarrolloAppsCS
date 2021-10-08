using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrograAsincrona
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //Ejemplo();
            _=Metodo1();
            //Al ser asincrono, no se espera y continua con las siguientes lineas

            //Tardate lo que tu quieras, yo seguire con las siguientes indicaciones
            await Metodo2();
            this.Text = "Termino";

            //Este es sincrono, los metodos me dan tiempo, pero este no asi que primero termino esta tarea
            // y luego me voy con los metodos

            //Crea un nuevo subproceso
            _ = Task.Run(() =>
            {
                long resultado = 0;
                for (int i = 0; i < 100000; i++)
                {
                    resultado += i;
                }
                // El subproceso no creó el textbox, asi que no puede acceder al textbox
                // El textbox se encuentra en un proceso diferente

                //Llamo al primer proceso que lo cambie por mi
                //this.Invoke(new prueba(CambiarText), resultado.ToString());
                Invoke(new Action<string>(CambiarText), resultado.ToString());
                // SI se ejecuta ya que ejecuta en el espacio de memoria de this, donde this es el formulario
                // Delegado Func no es void, tienes que especificar algo de entrada y de salida
                // Delegado Action puede ser algo de entrada pero no regresa nada
                // que es una referencia al hilo principal
            });
        }

        public delegate void prueba(string x);

        private void CambiarText(string a)
        {
            textBox1.Text = a ;
        }

       // public delegate void prueba();
       /*
        void Ejemplo()
        {
            var x = new prueba(metodo3); // un delegado es un apuntador a un espacio de codigo (referencia a metodos)
            x();
        }*/
        void metodo3()
        {
            MessageBox.Show("Hola");
        }
        private async Task Metodo1()
        {
            for (int i = 0; i < 1000; i++)
            {
                listBox1.Items.Add(i); // Agregalo
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                await Task.Delay(1); // Despues de agregarlo espera un milisegundo
                //Thread.Sleep(1); // Da una pausa a todo general del programa
            }
        }

        private async Task Metodo2()
        {
            for (int i = 1000; i < 2000; i++)
            {
                listBox2.Items.Add(i);
                listBox2.SelectedIndex = listBox2.Items.Count - 1;
                await Task.Delay(1);
            }
        }
    }
}
