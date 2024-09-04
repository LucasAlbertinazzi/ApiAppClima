using System;
using System.Collections.Generic;

namespace ApiAppClima.Models;

public partial class TblUsuario
{
    public int Codusuario { get; set; }

    public string? Nome { get; set; }

    public string? Senha { get; set; }
}
