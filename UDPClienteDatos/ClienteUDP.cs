using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPClienteDatos
{
    public class ClienteUDP
    {
        UdpClient cliente = new UdpClient() { EnableBroadcast = true};

        public void Enviar(string nombre, string lista)
        {
            string datos = $"{nombre}|{lista}";
            byte[] buffer = Encoding.UTF8.GetBytes(datos);

            cliente.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, 30002));
        }
    }
}
