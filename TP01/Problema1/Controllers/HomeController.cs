using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TP_Nro1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NLog;

namespace TP_Nro1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into HomeController");
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Hello, this is the index!");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public string Problema01(string numero)
        {
            string mensaje;
            int resultado;

            if (int.TryParse(numero, out int num))
            {
                try
                {
                    resultado = checked(num * num);
                    mensaje = resultado.ToString();
                }
                catch (Exception ex)
                {
                    mensaje = "El resultado esta fuera del rango esperado";

                    var mensajeLog = "Error message: " + ex.Message;
                    if (ex.InnerException != null)
                    {
                        mensajeLog = mensajeLog + " Inner exception: " + ex.InnerException.Message;
                    }
                    mensajeLog += " Stack Trace: " + ex.StackTrace;

                    _logger.LogError(mensajeLog);
                }
            }
            else
            {
                mensaje = "El dato ingresado no es un número o esta fuera del rango esperado";
            }         

            return mensaje;
        }

        public string Problema02(string dividendo, string divisor)
        {
            string resultado;

            if (float.TryParse(dividendo, out float a) && float.TryParse(divisor, out float b))
            {
                if (b != 0)
                {
                    resultado = (a / b).ToString();
                }
                else
                {
                    resultado = "No se puede dividir en cero";
                }
            }
            else
            {
                resultado = "El dato ingresado no es un número o esta fuera del rango esperado";
            }

            return resultado;
        }

        public string Problema03()
        {
            try
            {
                ProvinciasArgentina provinciasArg = ObtieneProvincias();
                string listaProvincias = "";

                foreach (Provincia provincia in provinciasArg.Provincias)
                {
                    listaProvincias += $"Nombre: {provincia.Nombre,-50} \t ID: {provincia.Id} \n";
                }

                return listaProvincias;
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex.ToString());

                return "Ocurrio un error al cargar la lista de provincias";                
            }
            
        }

        private static ProvinciasArgentina ObtieneProvincias()
        {

            var url = $"https://apis.datos.gob.ar/georef/api/provincias?campos=id,nombre";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            ProvinciasArgentina ProvinciasArg;
            ProvinciasArg = null;

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader != null)
                        {
                            using (StreamReader objReader = new StreamReader(strReader))
                            {
                                string responseBody = objReader.ReadToEnd();
                                ProvinciasArg = JsonSerializer.Deserialize<ProvinciasArgentina>(responseBody);
                            }
                        }
                    }
                }
            }
            catch (WebException)
            {
                throw;
            }
            return ProvinciasArg;
        }
        public string Problema04(string kilometros, string litros)
        {
            float resultado, km, l;
            string mensaje;

            try
            {
                if (kilometros != null && litros != null)
                {
                    km = float.Parse(kilometros, System.Globalization.NumberStyles.Float);
                    l = float.Parse(litros, System.Globalization.NumberStyles.Float);

                    if (l != 0)
                    {
                        resultado = km / l;
                        mensaje = resultado.ToString();
                    }
                    else
                    {
                        mensaje = "No se puede dividir en 0";
                    }
                }
                else
                {
                    mensaje = "Ingrese los datos antes de calcular";
                }
            }
            catch (FormatException ex)
            {
                mensaje = "Los datos ingresados no son numeros o no están en el formato correcto";
                _logger.LogError(ex.ToString());
            }
            catch (Exception ex)
            {
                mensaje = "Valores ingresados fuera de los rangos esperados";
                _logger.LogError(ex.ToString());
            }

            return mensaje;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }    

    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Parametros
    {
        [JsonPropertyName("campos")]
        public List<string> Campos { get; set; }
    }

    public class Provincia
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }
    }

    public class ProvinciasArgentina
    {
        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }

        [JsonPropertyName("inicio")]
        public int Inicio { get; set; }

        [JsonPropertyName("parametros")]
        public Parametros Parametros { get; set; }

        [JsonPropertyName("provincias")]
        public List<Provincia> Provincias { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
