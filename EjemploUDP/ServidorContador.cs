using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EjemploUDP
{
    public class ServidorContador
    {
        public int Valor { get; set; } = 0; // Crear propiedad 
        public event Action ValorCambio; //Se crea este evento para lanzar cuando haya llegado un cambio, se incrementa el valor cuando reciba un valor
        // Si fuera WPF podria usar Binding
        UdpClient listener;
        public bool iniciado;

        public void Iniciar()
        {
            if (!iniciado)
            {
                iniciado = true;
                Task.Run(IniciarServidor);
            }
            else
            { 
                
                Detener();
            }
            
        }

        private void IniciarServidor() // Iniciar el servidor
        {
            try
            {
                int puerto = 45001;//hay 65536 puertos 
                                   // Si el puerto asignado esta ocupado por otro programa o servicio causaria una excepcion
                                   // IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), puerto);
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, puerto);
                // Ahora hay que poner a escuchar el puerto
                listener = new UdpClient(puerto); //UdpClient(puerto) indica el Bind que separa el puerto
                while (iniciado)
                {
                    byte[] buffer = listener.Receive(ref ep); //El programa seguira esperando y no avanza hasta que reciba algo
                    if (buffer.Length > 0)
                    {
                        Valor++;
                        ValorCambio();
                        //ThreadSafeEventLaunch()
                    }
                }
            }
            catch (SocketException)
            {

            }
        }
        public void Detener()
        {
            if (listener!=null && iniciado)
            {
                iniciado = false;
                listener.Close();
                listener = null;
            }
        }

        void ThreadSafeEventLauch() //Lanzar evento con seguridad de hilo
        {
            if (ValorCambio!=null) // Si hay alguien suscrito
            {
                //ValorCambio.GetInvocationList()//Obtendria toda la listas del evento
                foreach (var d in ValorCambio.GetInvocationList())
                {
                    ISynchronizeInvoke i = d.Target as ISynchronizeInvoke; // El destino se convirrtio a Invoke
                    i.Invoke(ValorCambio, null);
                }
            }
        }
    }
}
