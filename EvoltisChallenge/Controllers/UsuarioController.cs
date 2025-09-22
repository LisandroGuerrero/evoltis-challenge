using EvoltisChallenge.DTOs;
using EvoltisChallenge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EvoltisChallenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario con sus datos de domicilio si los tiene</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetById(int id)
        {
            try
            {
                var usuario = await _service.GetByIdAsync(id);
                if (usuario == null)
                    return NotFound($"Usuario con ID {id} no encontrado");

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca usuarios por nombre y/o provincia/ciudad
        /// </summary>
        /// <param name="searchDto">Criterios de b�squeda</param>
        /// <returns>Lista de usuarios que coinciden con los criterios</returns>
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> Search([FromBody] SearchUsuarioDto searchDto)
        {
            try
            {
                var usuarios = await _service.SearchAsync(searchDto);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="usuarioDto">Datos del usuario a crear</param>
        /// <returns>Usuario creado</returns>
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> Create([FromBody] UsuarioDto usuarioDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuario = await _service.CreateAsync(usuarioDto);
                return CreatedAtAction(nameof(GetById), new { id = usuario.ID }, usuario);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="id">ID del usuario a actualizar</param>
        /// <param name="usuarioDto">Datos actualizados del usuario</param>
        /// <returns>Usuario actualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> Update(int id, [FromBody] UpdateUsuarioDto usuarioDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuario = await _service.UpdateAsync(id, usuarioDto);
                return Ok(usuario);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>Resultado de la operaci�n</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                    return NotFound($"Usuario con ID {id} no encontrado");

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }

}
