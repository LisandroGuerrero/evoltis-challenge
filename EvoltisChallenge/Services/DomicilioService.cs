using AutoMapper;
using EvoltisChallenge.Data.Repositories.Interfaces;
using EvoltisChallenge.DTOs;
using EvoltisChallenge.Models;
using EvoltisChallenge.Services.Interfaces;

namespace EvoltisChallenge.Services
{
    public class DomicilioService : IDomicilioService
    {
        private readonly IDomicilioRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public DomicilioService(
            IDomicilioRepository domicilioRepository,
            IUsuarioRepository usuarioRepository,
            IMapper mapper)
        {
            _repository = domicilioRepository;
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<DomicilioResponseDto> CreateAsync(int usuarioId, DomicilioDto domicilioDto)
        {
            // Verificar que el usuario existe
            if (!await _usuarioRepository.ExistsAsync(usuarioId))
                throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado");

            // Verificar que el usuario no tenga ya un domicilio (relación 1:1)
            var existingDomicilio = await _repository.GetByUsuarioIdAsync(usuarioId);
            if (existingDomicilio != null)
                throw new InvalidOperationException($"El usuario {usuarioId} ya tiene un domicilio asignado");

            // Validar que todos los campos estén completos
            ValidarDomicilioCompleto(domicilioDto);

            var domicilio = _mapper.Map<Domicilio>(domicilioDto);
            domicilio.UsuarioID = usuarioId;

            var createdDomicilio = await _repository.CreateAsync(domicilio);
            return _mapper.Map<DomicilioResponseDto>(createdDomicilio);
        }

        public async Task<bool> DeleteAsync(int usuarioId)
        {
            if (!await _usuarioRepository.ExistsAsync(usuarioId))
                throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado");

            var existingDomicilio = await _repository.GetByUsuarioIdAsync(usuarioId);
            if (existingDomicilio == null)
                throw new KeyNotFoundException($"El usuario {usuarioId} no tiene domicilio para eliminar");

            return await _repository.DeleteByUsuarioIdAsync(usuarioId);
        }

        public async Task<DomicilioResponseDto?> GetByUsuarioIdAsync(int usuarioId)
        {
            if (!await _usuarioRepository.ExistsAsync(usuarioId))
                throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado");

            var domicilio = await _repository.GetByUsuarioIdAsync(usuarioId);
            return domicilio == null ? null : _mapper.Map<DomicilioResponseDto>(domicilio);
        }

        public async Task<DomicilioResponseDto> UpdateAsync(int usuarioId, DomicilioDto domicilioDto)
        {
            if (!await _usuarioRepository.ExistsAsync(usuarioId))
                throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado");

            var domicilioExistente = await _repository.GetByUsuarioIdAsync(usuarioId);
            if (domicilioExistente == null)
            {
                domicilioExistente = new Domicilio
                {
                    UsuarioID = usuarioId,
                    FechaCreacion = DateTime.UtcNow
                };
            }

            // Actualizar los campos
            domicilioExistente.Calle = domicilioDto.Calle;
            domicilioExistente.Numero = domicilioDto.Numero;
            domicilioExistente.Provincia = domicilioDto.Provincia;
            domicilioExistente.Ciudad = domicilioDto.Ciudad;

            var domicilioActualizado = await _repository.UpdateAsync(domicilioExistente);
            return _mapper.Map<DomicilioResponseDto>(domicilioActualizado);
        }

        public void ValidarDomicilioCompleto(DomicilioDto domicilioDto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(domicilioDto.Calle))
                errors.Add("La calle es requerida");

            if (string.IsNullOrWhiteSpace(domicilioDto.Numero))
                errors.Add("El número es requerido");

            if (string.IsNullOrWhiteSpace(domicilioDto.Provincia))
                errors.Add("La provincia es requerida");

            if (string.IsNullOrWhiteSpace(domicilioDto.Ciudad))
                errors.Add("La ciudad es requerida");

            if (errors.Any())
            {
                throw new ArgumentException($"Domicilio incompleto: {string.Join(", ", errors)}");
            }
        }
    }
}
