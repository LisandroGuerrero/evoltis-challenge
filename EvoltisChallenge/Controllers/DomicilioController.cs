using EvoltisChallenge.DTOs;
using EvoltisChallenge.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EvoltisChallenge.Controllers
{
    [ApiController]
    [Route("api/{usuarioId}/[controller]")]
    public class DomicilioController : ControllerBase
    {
        private readonly IDomicilioService _service;

        public DomicilioController(IDomicilioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene el domicilio de un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Domicilio del usuario</returns>
        [HttpGet]
        public async Task<ActionResult<DomicilioResponseDto>> GetByUsuarioId(int usuarioId)
        {
            try
            {
                var domicilio = await _service.GetByUsuarioIdAsync(usuarioId);
                if (domicilio == null)
                    return NotFound($"El usuario {usuarioId} no tiene domicilio");

                return Ok(domicilio);
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

        /// <summary>
        /// Crea un domicilio para un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="domicilioDto">Datos del domicilio</param>
        /// <returns>Domicilio creado</returns>
        [HttpPost]
        public async Task<ActionResult<DomicilioResponseDto>> Create(int usuarioId, [FromBody] DomicilioDto domicilioDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var domicilio = await _service.CreateAsync(usuarioId, domicilioDto);
                return CreatedAtAction(nameof(GetByUsuarioId), new { usuarioId }, domicilio);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        /// Actualiza el domicilio de un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="domicilioDto">Datos actualizados del domicilio</param>
        /// <returns>Domicilio actualizado</returns>
        [HttpPut]
        public async Task<ActionResult<DomicilioResponseDto>> Update(int usuarioId, [FromBody] DomicilioDto domicilioDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var domicilio = await _service.UpdateAsync(usuarioId, domicilioDto);
                return Ok(domicilio);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        /// Elimina el domicilio de un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int usuarioId)
        {
            try
            {
                var result = await _service.DeleteAsync(usuarioId);
                if (!result)
                    return NotFound($"El usuario {usuarioId} no tiene domicilio para eliminar");

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
