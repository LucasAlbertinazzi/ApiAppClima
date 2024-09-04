using ApiAppClima.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiAppClima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly PostgresContext _dbContext;

        public UsuariosController(PostgresContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("cadastro-usuario")]
        public IActionResult CadastrarUsuario([FromBody] TblUsuario novoUsuario)
        {
            if (_dbContext.TblUsuarios.Any(u => u.Nome == novoUsuario.Nome))
            {
                return BadRequest("Usuário já cadastrado.");
            }

            _dbContext.TblUsuarios.Add(novoUsuario);

            _dbContext.SaveChanges();

            return Ok("Usuário cadastrado com sucesso.");
        }

        [HttpPost]
        [Route("autenticacao")]
        public int AutenticarUsuario([FromBody] TblUsuario usuarioLogin)
        {
            var usuario = _dbContext.TblUsuarios.FirstOrDefault(u => u.Nome == usuarioLogin.Nome && u.Senha == usuarioLogin.Senha);

            if (usuario != null)
            {
                return usuario.Codusuario;
            }

            return 0;
        }

        [HttpGet]
        [Route("todos-usuarios")]
        public ActionResult<IEnumerable<TblUsuario>> ObterTodosUsuarios()
        {
            var usuarios = _dbContext.TblUsuarios.ToList();
            if (usuarios == null || !usuarios.Any())
            {
                return NotFound("Nenhum usuário encontrado.");
            }
            return Ok(usuarios);
        }
    }
}
