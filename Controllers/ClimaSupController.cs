using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ApiAppClima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClimaSupController : ControllerBase
    {
        private static readonly string apiKey = "fbe0a838653dd1efac1c9e85b1e2cc5e";
        private static readonly string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        // GET api/ClimaSup/cidade
        [HttpGet("{cidade}")]
        public async Task<IActionResult> BuscarClima(string cidade)
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

                        string temperatura = json["main"]["temp"].ToString();
                        string descricao = json["weather"][0]["description"].ToString();

                        // Retorna o resultado em formato JSON
                        return Ok(new
                        {
                            Cidade = cidade,
                            Temperatura = $"{temperatura}°C",
                            Descricao = descricao
                        });
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
    }
}
