using AutoMapper;
using EvoltisChallenge.Data.Repositories.Interfaces;
using EvoltisChallenge.DTOs;
using EvoltisChallenge.Models;
using EvoltisChallenge.Services;
using EvoltisChallenge.Services.Interfaces;
using EvoltisChallenge.Services.Mappings;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Tests
{
    public class UnitTest
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IDomicilioRepository> _mockDomicilioRepository;
        private readonly Mock<IDomicilioService> _mockDomicilioService;
        private readonly NullLoggerFactory _loggerFactory;
        private readonly IMapper _mapper;
        private readonly UsuarioService _usuarioService;

        public UnitTest()
        {
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockDomicilioRepository = new Mock<IDomicilioRepository>();
            _mockDomicilioService = new Mock<IDomicilioService>();

            _loggerFactory = new NullLoggerFactory();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>(), _loggerFactory);
            _mapper = mapperConfig.CreateMapper();

            _usuarioService = new UsuarioService(
                _mockUsuarioRepository.Object,
                _mockDomicilioRepository.Object,
                _mockDomicilioService.Object,
                _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingUser_ReturnsUserDto()
        {
            var userId = 1;
            var usuario = new Usuario
            {
                ID = userId,
                Nombre = "Jerónimo Bugao",
                Email = "jib@test.com",
                FechaCreacion = DateTime.UtcNow,
                Domicilio = new Domicilio
                {
                    ID = 1,
                    UsuarioID = userId,
                    Calle = "Av Montevideo",
                    Numero = "1092",
                    Provincia = "Córdoba",
                    Ciudad = "Capital",
                    FechaCreacion = DateTime.UtcNow
                }
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(usuario);

            var result = await _usuarioService.GetByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.ID);
            Assert.Equal("Jerónimo Bugao", result.Nombre);
            Assert.Equal("jib@test.com", result.Email);
            Assert.NotNull(result.Domicilio);
            Assert.Equal("Av Montevideo", result.Domicilio.Calle);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingUser_ReturnsNull()
        {
            var userId = 999;
            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((Usuario?)null);

            var result = await _usuarioService.GetByIdAsync(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ValidUser_ReturnsCreatedUser()
        {
            var usuarioDto = new UsuarioDto
            {
                Nombre = "Rodrigo García",
                Email = "rodriwheels@test.com"
            };

            var usuarioCreado = new Usuario
            {
                ID = 1,
                Nombre = usuarioDto.Nombre,
                Email = usuarioDto.Email,
                FechaCreacion = DateTime.UtcNow
            };

            _mockUsuarioRepository.Setup(x => x.EmailExistsAsync(usuarioDto.Email, null)).ReturnsAsync(false);
            _mockUsuarioRepository.Setup(x => x.CreateAsync(It.IsAny<Usuario>())).ReturnsAsync(usuarioCreado);

            var result = await _usuarioService.CreateAsync(usuarioDto);

            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal("Rodrigo García", result.Nombre);
            Assert.Equal("rodriwheels@test.com", result.Email);
        }

        [Fact]
        public async Task CreateAsync_DuplicateEmail_ThrowsInvalidOperationException()
        {
            var usuarioDto = new UsuarioDto
            {
                Nombre = "Jerónimo Duplicado",
                Email = "jib.duplicado@test.com"
            };

            _mockUsuarioRepository.Setup(x => x.EmailExistsAsync(usuarioDto.Email, null)).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _usuarioService.CreateAsync(usuarioDto));

            Assert.Contains("Ya existe un usuario con el email", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsUpdatedUser()
        {
            var userId = 1;
            var updateDto = new UpdateUsuarioDto
            {
                Nombre = "Jerónimo Modificado",
                Email = "jib.modificado@test.com"
            };

            var existingUsuario = new Usuario
            {
                ID = userId,
                Nombre = "Jerónimo Original",
                Email = "jib@test.com",
                FechaCreacion = DateTime.UtcNow.AddDays(-1)
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(existingUsuario);
            _mockUsuarioRepository.Setup(x => x.EmailExistsAsync(updateDto.Email, userId)).ReturnsAsync(false);
            _mockUsuarioRepository.Setup(x => x.UpdateAsync(It.IsAny<Usuario>())).ReturnsAsync(existingUsuario);

            var result = await _usuarioService.UpdateAsync(userId, updateDto);

            Assert.NotNull(result);
            Assert.Equal("Jerónimo Modificado", result.Nombre);
            Assert.Equal("jib.modificado@test.com", result.Email);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingUser_ThrowsKeyNotFoundException()
        {
            var userId = 999;
            var updateDto = new UpdateUsuarioDto
            {
                Nombre = "Usuario Inexistente",
                Email = "inexistente@test.com"
            };

            _mockUsuarioRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((Usuario?)null);

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _usuarioService.UpdateAsync(userId, updateDto));

            Assert.Contains($"Usuario con ID {userId} no encontrado", exception.Message);
        }

        [Fact]
        public async Task DeleteAsync_ExistingUser_ReturnsTrue()
        {
            var userId = 1;
            _mockUsuarioRepository.Setup(x => x.ExistsAsync(userId)).ReturnsAsync(true);
            _mockUsuarioRepository.Setup(x => x.DeleteAsync(userId)).ReturnsAsync(true);

            var result = await _usuarioService.DeleteAsync(userId);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingUser_ThrowsKeyNotFoundException()
        {
            var userId = 999;
            _mockUsuarioRepository.Setup(x => x.ExistsAsync(userId)).ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _usuarioService.DeleteAsync(userId));

            Assert.Contains($"Usuario con ID {userId} no encontrado", exception.Message);
        }

        [Fact]
        public async Task SearchAsync_WithFilters_ReturnsFilteredUsers()
        {
            var searchDto = new SearchUsuarioDto
            {
                Nombre = "Catalina Paperas",
                Provincia = "Córdoba"
            };

            var usuarios = new List<Usuario>
            {
                new() {
                    ID = 1,
                    Nombre = "Catalina Paperas",
                    Email = "catapg@test.com",
                    FechaCreacion = DateTime.UtcNow,
                    Domicilio = new Domicilio
                    {
                        ID = 1,
                        UsuarioID = 1,
                        Provincia = "Córdoba",
                        Ciudad = "Capital",
                        Calle = "9 de Julio",
                        Numero = "3177",
                        FechaCreacion = DateTime.UtcNow
                    }
                }
            };

            _mockUsuarioRepository.Setup(x => x.SearchAsync(searchDto)).ReturnsAsync(usuarios);

            var result = await _usuarioService.SearchAsync(searchDto);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Catalina Paperas", result.First().Nombre);
        }
    }
}
