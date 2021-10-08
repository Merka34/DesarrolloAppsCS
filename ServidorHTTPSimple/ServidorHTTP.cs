using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServidorHTTPSimple
{
    public class ServidorHTTP
    {
        public HttpListener Cliente { get; set; }
        public event Action<string> NombreAgregado;

        public void Iniciar()
        {
            if (Cliente==null)
            {
                Cliente = new HttpListener();
                Cliente.Prefixes.Add("http://*:81/"); //Prefijos son las URIs
                //Cliente.Prefixes.Add("http://127.0.0.1:81/");
                //Cliente.Prefixes.Add("http://misitio.com/"); //Para comunicarse asi tendra que registrarse al DNS
                Cliente.Start();
                // EL protocolo HTTP es un protocolo de seguridad mas alto que TCP y UDP
                // Windows no permite el protocolo HTTP si no esta registrado al Firewall 
                // Si pone el puerto 80 si permitiria, otro puerto no.
                // Abra que correrlo con permiso de Administrador

                Task.Run(() => {
                    //Esperar a que se conecte alguien - Esperar un httpcontext
                    while (true)
                    {
                        var context = Cliente.GetContext(); // Espera por una peticion entrante y cuanto uno se recibe
                        if (context.Request.HttpMethod == "GET" && context.Request.RawUrl == "/")
                        {
                            var ensamblado = Assembly.GetExecutingAssembly();
                            var stream = ensamblado.GetManifestResourceStream("ServidorHTTPSimple.index.html");

                            //byte[] buffer = Encoding.UTF8.GetBytes("Hola");
                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            context.Response.ContentType = "text/html";
                            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                            context.Response.StatusCode = 200; //ok
                        }
                        else if (context.Request.HttpMethod=="POST" && context.Request.RawUrl=="/")
                        {
                            var bufferEntrada = new byte[context.Request.ContentLength64];
                            context.Request.InputStream.Read(bufferEntrada, 0, bufferEntrada.Length);
                            string datos = Encoding.UTF8.GetString(bufferEntrada);
                            var formdata = HttpUtility.ParseQueryString(datos);
                            NombreAgregado?.Invoke(formdata["nombre"]);
                        }
                        else
                            context.Response.StatusCode = 404; //Not Found
                        context.Response.Close();
                    }

                });
            }
        }

        public void Detener()
        {
            if (Cliente!=null)
            {
                Cliente.Stop();
                Cliente = null;
            }
        }
    }
}
