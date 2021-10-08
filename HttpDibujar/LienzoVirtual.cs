using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HttpDibujar
{
    public class LienzoVirtual
    {
        public ObservableCollection<Shape> Figuras { get; set; } = new ObservableCollection<Shape>();
        HttpListener servidor = new HttpListener();
        Dispatcher dispatcher;

        public LienzoVirtual()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            Iniciar();
           // Deserealizar();
        }
        public void Iniciar()
        {
            if (!servidor.IsListening)
            {
                servidor.Prefixes.Add("http://*:10001/");
                servidor.Start();

                Task.Run(RecibirConexion);
            }
        }

        void Detener()
        {
            if (servidor.IsListening)
            {

            }
        }

        void RecibirConexion() // Subproceso
        {
            var ensamblado = Assembly.GetExecutingAssembly();
            var stream = ensamblado.GetManifestResourceStream("HttpDibujar.index.html");
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            while (servidor.IsListening)
            {
                var context = servidor.GetContext();
                if (context.Request.RawUrl == "/")
                {
                    context.Response.ContentType = "text/html";
                    context.Response.OutputStream.Write(buffer);
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                }
                else if (context.Request.RawUrl == "/dibujar" && context.Request.HttpMethod == "POST")
                {
                    byte[] contenido = new byte[context.Request.ContentLength64];
                    context.Request.InputStream.Read(contenido, 0, contenido.Length);
                    var diccionario = HttpUtility.ParseQueryString(Encoding.UTF8.GetString(contenido));

                    dispatcher.Invoke(() =>
                    CrearShape(diccionario["figura"],
                        double.Parse(diccionario["altura"]),
                        double.Parse(diccionario["ancho"]),
                        diccionario["color"],
                        int.Parse(diccionario["x"]),
                        int.Parse(diccionario["y"]))
                    );

                    context.Response.Redirect("/");
                    context.Response.Close();
                }
                else
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                }
            }
        }

        void CrearShape(string figura, double altura, double ancho, string color, int x, int y)
        {
            Shape f = figura == "Rectángulo" ? new Rectangle() : new Ellipse();
            f.Width = ancho;
            f.Height = altura;
            f.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
            Canvas.SetLeft(f, x);
            Canvas.SetTop(f, y);
            Figuras.Add(f);
            //Serializar();
        }

        void Serializar()
        {
            string json = JsonConvert.SerializeObject(Figuras); 
            File.WriteAllText("figuras.json", json);
        }

        void Deserealizar()
        {
            if (File.Exists("figuras.json"))
            {
                var datos = File.ReadAllText("figuras.json");
                var lista = JsonConvert.DeserializeObject<List<Shape>>(datos);
                lista.ForEach(x => Figuras.Add(x));
            }
        }
    }
}
