using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ChatTCPServer
{
    public class TCPServer
    {
        TcpListener listener;
        List<TcpClient> clients = new List<TcpClient>();
        public ObservableCollection<string> Mensajes { get; set; } = new ObservableCollection<string>();

        public ICommand IniciarCommand { get; set; }
        public ICommand DetenerCommand { get; set; }

        Dispatcher dispatcher; // Realiza metodos fuera de su metodo y realice el Invoke

        public TCPServer()
        {
            dispatcher = Dispatcher.CurrentDispatcher; //Obtiene una referencia al proceso que se esta ejecutando actualmente
            IniciarCommand = new RelayCommand(Iniciar);
            DetenerCommand = new RelayCommand(Detener);
        }

        public void Iniciar()
        {
            if (listener==null)
            {
                Task.Run(() => {
                    try
                    {
                        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 5000);
                        listener = new TcpListener(ep);
                        listener.Start(); // Comienza a escuchar por el puerto 5000 a ver si alguien 
                                          //Aqui hace la pausa

                        while (listener != null)
                        {
                            //Recibe una solicitud de conexión
                            TcpClient tcp = listener.AcceptTcpClient(); //Los datos de la otra persona
                                                                        // Ahi se origina el endpoint remoto
                            clients.Add(tcp);
                            dispatcher.Invoke(() => {
                                Mensajes.Add($"Se ha conectado el cliente: {tcp.Client.RemoteEndPoint}");
                            });

                            Recibir(tcp);
                            //Pero ahora hay que poner a escuchar al cliente para estar pendiente de lo que mande
                        }
                    }
                    //WSACancelBlock in Call
                    //Windows Socket Application Cancel Block in Call
                    catch (Exception)
                    {

                    }
                });
            }
        }

        void Recibir(TcpClient client)
        {
            Task.Run(() =>
            {
                NetworkStream ns = client.GetStream(); // GetStream regresa NetworkStream el cual permite leer o mandar los datos
                while (client.Connected) //Si el cliente sigue conectado
                {
                    if (client.Available>0) //Si hay datos disponibles
                    {
                        //Leer los datos
                        byte[] buffer = new byte[client.Available];
                        ns.Read(buffer, 0, buffer.Length);
                        string mensaje = Encoding.UTF8.GetString(buffer);

                        //Cross thread call - Llamada cruzada entre hilos
                        dispatcher.Invoke(() =>
                        {
                            Mensajes.Add(mensaje);
                        });

                        //Reenviar a los clientes
                        // RELAY mensaje
                        foreach (var c in clients) //Recorrer todos los clientes de la lista
                        {
                            //Verificar que no sea el mismo cliente que envio y asegurar que el cliente que este conectado
                            if (c!=client && c.Connected)
                            {
                                var st = c.GetStream();
                                st.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    Task.Delay(100);
                }
            });
        }

        public void Detener()
        {
            if (listener!=null)
            {
                //Detener el listener
                listener.Stop();
                listener = null;
                //Desconectarse de los clientes
                foreach (var client in clients)
                {
                    client.Close();
                }
                //Asi mataria a todas los hilos que escuchan a los clientes
                clients.Clear(); //Eliminar todos los clientes
            }
        }
    }
}
