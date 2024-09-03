using System;
using System.Collections.Generic;

namespace ApiAppClima.Models;

public partial class TblUsuario
{
    public int Codusuario { get; set; }

    public string Nome { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public bool Status { get; set; }
}
