using System;
using System.Collections.Generic;

namespace ApiAppClima.Models;

public partial class TblHistClima
{
    public int Idhist { get; set; }

    public int? Coduser { get; set; }

    public string? Cidade { get; set; }

    public string? Temperatura { get; set; }

    public string? Descricao { get; set; }
}
