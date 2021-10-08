using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPServerDatos
{
    public class ServidorUDP
    {
        UdpClient client;
        public event Action<string> AgregadoLista1, AgregadoLista2;
        public bool iniciado;

        public void Iniciar()
        {
            try
            {
                if (client == null)
                {
                    iniciado = true;
                    Task.Run(() => {
                        int puerto = 30002;
                        IPEndPoint ip = new IPEndPoint(IPAddress.Any, puerto);
                        
                        client = new UdpClient(ip); //Binding reservar el puerto
                        client.EnableBroadcast = true;
                        while (true)
                        {
                            byte[] buffer = client.Receive(ref ip);
                            string dato = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                            var info = dato.Split('|');
                            if (info[1] == "1")
                            {
                                AgregadoLista1(info[0]);
                            }
                            else
                            {
                                AgregadoLista2(info[0]);
                            }
                        }
                    });
                }
            }
            catch (SocketException)
            {

            }
        }

        public void Detener()
        {
            try
            {
                if (client!=null)
                {
                    iniciado = false;
                    client.Close();
                    client = null;
                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}
