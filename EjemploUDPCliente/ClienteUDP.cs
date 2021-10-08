using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EjemploUDPCliente
{
    public class ClienteUDP
    {

        public void Enviar(IPAddress ip)
        {
            IPEndPoint remoto = new IPEndPoint(ip, 45001);
            UdpClient client = new UdpClient();
            client.Connect(remoto);

            byte[] datos = new byte[] { 1 };

            client.Send(datos, datos.Length);
        }
    }
}
