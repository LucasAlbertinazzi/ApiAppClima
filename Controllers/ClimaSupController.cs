using ApiAppClima.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace ApiAppClima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClimaSupController : ControllerBase
    {
        private static readonly string apiKey = "fbe0a838653dd1efac1c9e85b1e2cc5e";
        private static readonly string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        private readonly PostgresContext _dbContext;

        public ClimaSupController(PostgresContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("busca-clima")]
        public async Task<IActionResult> BuscarClima(string cidade, int codusuario)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{baseUrl}?q={cidade}&appid={apiKey}&units=metric";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(result);

                        // Extraindo todos os dados relevantes
                        var coordenadas = json["coord"];
                        var clima = json["weather"][0];
                        var main = json["main"];
                        var vento = json["wind"];
                        var nuvens = json["clouds"];
                        var sys = json["sys"];

                        // Traduzindo a descrição
                        string descricao = clima["description"].ToString();

                        // Criando o histórico de clima com mais detalhes
                        TblHistClima historico = new TblHistClima
                        {
                            Coduser = codusuario,
                            Cidade = cidade.ToUpper(),
                            Temperatura = $"{main["temp"]}°C",
                            TemperaturaMinima = $"{main["temp_min"]}°C",
                            TemperaturaMaxima = $"{main["temp_max"]}°C",
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

                        // Adicionando o histórico ao banco de dados
                        _dbContext.TblHistClimas.Add(historico);
                        await _dbContext.SaveChangesAsync();

                        // Retornando a classe Historico
                        return Ok(historico);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Erro ao buscar dados de clima.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Erro ao se comunicar com a API: {ex.Message}");
                }
            }
        }

        [HttpGet]
        [Route("ultimos-registros")]
        public async Task<ActionResult<List<TblHistClima>>> ObterUltimosRegistros(int codusuario)
        {
            var ultimosRegistros = await _dbContext.TblHistClimas
                .Where(h => h.Coduser == codusuario)
                .OrderByDescending(h => h.Idhist)
                .Take(5)
                .ToListAsync();

            if (ultimosRegistros == null || ultimosRegistros.Count == 0)
            {
                return NotFound("Nenhum registro encontrado para o usuário informado.");
            }

            return Ok(ultimosRegistros);
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
