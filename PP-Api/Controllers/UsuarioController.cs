using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP_Api.AuthError;
using PP_Api.Modelos;
using PP_Dominio.Entidades;
using PP_Servicios;

namespace PP_Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServicio usuarioServicio;
        private readonly IMapper mapper;

        public UsuarioController(IUsuarioServicio usuarioServicio, IMapper mapper)
        {
            this.usuarioServicio = usuarioServicio;
            this.mapper = mapper;
        }

        #region Metodos de busqueda

        // GET: api/<UsuarioController>/getusuario
        [HttpGet("getusuario")]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Validamos la autenticidad del token
                if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

                IEnumerable<Usuario> usuarios = await usuarioServicio.GetAll();

                IEnumerable<UsuarioModel> usuarioModels = mapper.Map<IEnumerable<UsuarioModel>>(usuarios);

                return Ok(new { total = usuarioModels.Count(), usuarioModels });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch
            {
                return BadRequest(); // debemos poner un mensaje
            }
        }

        #endregion

        #region Metodos de persistencia
        // POST: api/<UsuarioController>/postusuario
        [HttpPost("postusuario")]
        public async Task<IActionResult> Post([FromBody] UsuarioModel model)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            // Verificamos si existe usuario con el UserName
            if (usuarioServicio.VeryfyUserNameExists(model.UserName))
                return NotFound(new { mensaje = "UserName existente" });

            Usuario usuario = mapper.Map<Usuario>(model);
            
            await usuarioServicio.Create(usuario);
            return Ok(new { mensaje = "Usuario creado con exito" });
        }

        // PUT: api/<UsuarioController>/putusuario/5
        [HttpPut("putusuario/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioModel model)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            // Verificamos
            if (!usuarioServicio.VeryfyIdExists(id))
                return NotFound(new { mensaje = "Usuario inexistente." });

            Usuario usuario = mapper.Map<Usuario>(model);

            await usuarioServicio.Update(id, usuario);
            return Ok(new { mensaje = "Usuario actualizado con exito" });
        }

        // DELETE: api/<UsuarioController>/deleteusuario/5
        [HttpDelete("deleteusuario/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Validamos la autenticidad del token
            if (!User.Identity?.IsAuthenticated ?? false) return Unauthorized(new { mensaje = AuthMessageError.UnauthorizedMessage() });

            // Verificamos
            if (!usuarioServicio.VeryfyIdExists(id))
                return NotFound(new { mensaje = "Usuario inexistente." });

            await usuarioServicio.Delete(id);
            return Ok(new { mensaje = "Usuario eliminado con exito" });
        }

        #endregion
    }
}
