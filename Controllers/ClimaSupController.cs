using ApiAppClima.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiAppClima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClimaSupController : ControllerBase
    {
        private static readonly string apiKey = "fbe0a838653dd1efac1c9e85b1e2cc5e";
        private static readonly string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        private readonly PostgresContext _dbContext;
        private readonly HttpClient _httpClient;

        public ClimaSupController(PostgresContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
        }

        [HttpGet]
        [Route("busca-clima")]
        public async Task<TblHistClima> BuscarClima(string cidade, int codusuario)
        {
            string url = $"{baseUrl}?q={cidade}&appid={apiKey}&units=metric";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);

                    var coordenadas = json["coord"];
                    var clima = json["weather"][0];
                    var main = json["main"];
                    var vento = json["wind"];
                    var nuvens = json["clouds"];
                    var sys = json["sys"];

                    var descricao = clima["description"].ToString();

                    var tempMin = Math.Round((double)main["temp_min"] - 1, 2);
                    var tempMax = Math.Round((double)main["temp_max"] + 1, 2);

                    TblHistClima historico = new TblHistClima
                    {
                        Coduser = codusuario,
                        Cidade = cidade.ToUpper(),
                        Temperatura = $"{main["temp"]}°C",
                        TemperaturaMinima = $"{tempMin}°C",
                        TemperaturaMaxima = $"{tempMax}°C",
                        Pressao = $"{main["pressure"]} hPa",
                        Umidade = $"{main["humidity"]}%",
                        VelocidadeVento = vento["speed"].ToString(),
                        DirecaoVento = vento["deg"].ToString(),
                        Descricao = descricao,
                        Nuvens = nuvens["all"].ToString(),
                        Latitude = (double)coordenadas["lat"],
                        Longitude = (double)coordenadas["lon"],
                        Visibilidade = json["visibility"].ToString(),
                        NascerDoSol = DateTimeOffset.FromUnixTimeSeconds((long)sys["sunrise"]).UtcDateTime,
                        PorDoSol = DateTimeOffset.FromUnixTimeSeconds((long)sys["sunset"]).UtcDateTime,
                        DataHora = DateTimeOffset.FromUnixTimeSeconds((long)json["dt"]).UtcDateTime
                    };

                    _dbContext.TblHistClimas.Add(historico);
                    await _dbContext.SaveChangesAsync();

                    return historico;
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("ultimos-registros")]
        public async Task<List<TblHistClima>> ObterUltimosRegistros(int codusuario)
        {
            var ultimosRegistros = await _dbContext.TblHistClimas
                .Where(h => h.Coduser == codusuario)
                .OrderByDescending(h => h.Idhist)
                .Take(5)
                .ToListAsync();

            if (ultimosRegistros == null || ultimosRegistros.Count == 0)
            {
                return null;
            }

            return ultimosRegistros;
        }


        [HttpGet]
        [Route("historico-horario")]
        public async Task<IActionResult> BuscarHistoricoPorHora(double lat, double lon, long start, long end)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://history.openweathermap.org/data/2.5/history/city?lat={lat}&lon={lon}&type=hour&start={start}&end={end}&appid={apiKey}";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(result);

                        var historico = json["list"].Select(h => new
                        {
                            DataHora = DateTimeOffset.FromUnixTimeSeconds((long)h["dt"]).UtcDateTime,
                            Temperatura = (double)h["main"]["temp"] - 273.15,
                            SensacaoTermica = (double)h["main"]["feels_like"] - 273.15,
                            Pressao = (int)h["main"]["pressure"],
                            Umidade = (int)h["main"]["humidity"],
                            TemperaturaMinima = (double)h["main"]["temp_min"] - 273.15,
                            TemperaturaMaxima = (double)h["main"]["temp_max"] - 273.15,
                            VelocidadeVento = (double)h["wind"]["speed"],
                            DirecaoVento = (int)h["wind"]["deg"],
                            Nuvens = (int)h["clouds"]["all"],
                            DescricaoClima = (string)h["weather"][0]["description"]
                        }).ToList();

                        return Ok(historico);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Erro ao buscar dados históricos.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Erro ao se comunicar com a API: {ex.Message}");
                }
            }
        }
    }
}
