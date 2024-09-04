using System;
using System.Collections.Generic;

namespace ApiAppClima.Models;

public partial class TblHistClima
{
    public int Idhist { get; set; }

    public int? Coduser { get; set; }

    public string? Cidade { get; set; }

    public string? Temperatura { get; set; }

    public string? TemperaturaMinima { get; set; }

    public string? TemperaturaMaxima { get; set; }

    public string? Pressao { get; set; }

    public string? Umidade { get; set; }

    public string? Descricao { get; set; }

    public string? VelocidadeVento { get; set; }

    public string? DirecaoVento { get; set; }

    public string? Nuvens { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Visibilidade { get; set; }

    public DateTime? NascerDoSol { get; set; }

    public DateTime? PorDoSol { get; set; }

    public DateTime? DataHora { get; set; }
}
