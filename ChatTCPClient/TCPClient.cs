using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ChatTCPClient
{
    public class TCPClient : INotifyPropertyChanged
    {
        TcpClient client = new TcpClient();
        public ObservableCollection<string> Mensajes { get; set; } = new();
        private string username;
        Dispatcher dispatcher;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        private string ip;

        public string ServerIP
        {
            get { return ip; }
            set { ip = value; }
        }
        private bool conectado = false;

        public bool Conectado
        {
            get { return conectado; }
            set { conectado = value; }
        }

        public ICommand ConectarCommand { get; set; }
        public ICommand EnviarCommand { get; set; }

        public TCPClient()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            ConectarCommand = new RelayCommand(Conectar);
            EnviarCommand = new RelayCommand(Enviar);
        }

        private void Conectar()
        {
            if (!Conectado)
            {
                //Validar
                client.Connect(new IPEndPoint(IPAddress.Parse(ServerIP), 5000));
                Task.Delay(10);
                Conectado = client.Connected;

                //Iniciar un hilo para escuchar
                Task.Run(() => { Recibir(); }); //Iniciar un subproceso para recibir mensajes
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Conectado)));
            }
        }

        private void Recibir()
        {
            var ns = client.GetStream();
            while (client.Connected)
            {
                if (client.Available>0)
                {
                    var buffer = new byte[client.Available];
                    ns.Read(buffer, 0, buffer.Length);

                    var mensaje = Encoding.UTF8.GetString(buffer);

                    dispatcher.Invoke(() => {
                        Mensajes.Add(mensaje);
                    });
                }
                Task.Delay(10);
            }
        }

        public string Mensaje { get; set; }
        private void Enviar()
        {
            if (!string.IsNullOrWhiteSpace(Mensaje))
            {
                NetworkStream ns = client.GetStream();
                var buffer = Encoding.UTF8.GetBytes($"{Username}: {Mensaje}");
                ns.Write(buffer, 0, buffer.Length);
                Mensajes.Add(Mensaje);
                Mensaje = "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
