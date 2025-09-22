using AutoMapper;
using EvoltisChallenge.Data.Repositories.Interfaces;
using EvoltisChallenge.DTOs;
using EvoltisChallenge.Models;
using EvoltisChallenge.Services.Interfaces;

namespace EvoltisChallenge.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IDomicilioRepository _domicilioRepository;
        private readonly IDomicilioService _domicilioService;
        private readonly IMapper _mapper;

        public UsuarioService(
            IUsuarioRepository userRepository,
            IDomicilioRepository domicilioRepository,
            IDomicilioService domicilioService,
            IMapper mapper)
        {
            _repository = userRepository;
            _domicilioRepository = domicilioRepository;
            _domicilioService = domicilioService;
            _mapper = mapper;
        }

        public async Task<UsuarioResponseDto> CreateAsync(UsuarioDto usuarioDto)
        {
            if (await _repository.EmailExistsAsync(usuarioDto.Email))
                throw new InvalidOperationException($"Ya existe un usuario con el email {usuarioDto.Email}");

            var usuario = _mapper.Map<Usuario>(usuarioDto);
            var createdUsuario = await _repository.CreateAsync(usuario);

            return _mapper.Map<UsuarioResponseDto>(createdUsuario);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (!await _repository.ExistsAsync(id))
                throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");

            return await _repository.DeleteAsync(id);
        }

        public async Task<UsuarioResponseDto?> GetByIdAsync(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            return usuario == null ? null : _mapper.Map<UsuarioResponseDto>(usuario);
        }

        public async Task<IEnumerable<UsuarioResponseDto>> SearchAsync(SearchUsuarioDto searchDto)
        {
            var usuarios = await _repository.SearchAsync(searchDto);
            return _mapper.Map<IEnumerable<UsuarioResponseDto>>(usuarios);
        }

        public async Task<UsuarioResponseDto> UpdateAsync(int id, UpdateUsuarioDto usuarioDto)
        {
            var usuarioExistente = await _repository.GetByIdAsync(id);
            if (usuarioExistente == null)
                throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");

            if (await _repository.EmailExistsAsync(usuarioDto.Email, id))
                throw new InvalidOperationException($"Ya existe otro usuario con el email {usuarioDto.Email}");

            usuarioExistente.Nombre = string.IsNullOrWhiteSpace(usuarioDto.Nombre) ? usuarioExistente.Nombre : usuarioDto.Nombre;
            usuarioExistente.Email = string.IsNullOrWhiteSpace(usuarioDto.Email) ? usuarioExistente.Email : usuarioDto.Email;

            if (usuarioDto.Domicilio != null)
                await _domicilioService.UpdateAsync(id, usuarioDto.Domicilio);

            var usuarioActualizado = await _repository.UpdateAsync(usuarioExistente);
            return _mapper.Map<UsuarioResponseDto>(usuarioActualizado);
        }
    }
}
