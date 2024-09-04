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
        public IActionResult AutenticarUsuario([FromBody] TblUsuario usuarioLogin)
        {
            var usuario = _dbContext.TblUsuarios.FirstOrDefault(u => u.Nome == usuarioLogin.Nome && u.Senha == usuarioLogin.Senha);

            if (usuario != null)
            {
                return Ok("Autenticação bem-sucedida.");
            }
            else
            {
                return Unauthorized("Nome ou senha inválidos.");
            }
        }
    }
}
