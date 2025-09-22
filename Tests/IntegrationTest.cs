using EvoltisChallenge.Data;
using EvoltisChallenge.DTOs;
using EvoltisChallenge.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Tests
{
    public class IntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public IntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // 1. Crear conexión SQLite en memoria
                        var connection = new SqliteConnection("DataSource=:memory:");
                        connection.Open();

                        // 2. Reemplazar DbContext
                        var descriptor = services.Single(
                            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                        services.Remove(descriptor);

                        services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlite(connection));

                        // 3. Crear las tablas
                        using var scope = services.BuildServiceProvider().CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        db.Database.EnsureCreated();
                    });
                });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateUsuario_ValidData_ReturnsCreatedUser()
        {
            var usuarioDto = new UsuarioDto
            {
                Nombre = "Test Usuario",
                Email = "test@example.com"
            };

            var response = await _client.PostAsJsonAsync("/api/Usuario", usuarioDto);

            response.EnsureSuccessStatusCode();

            var usuarioResponse = await response.Content.ReadFromJsonAsync<UsuarioResponseDto>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(usuarioResponse);
            Assert.Equal(usuarioDto.Nombre, usuarioResponse!.Nombre);
            Assert.Equal(usuarioDto.Email, usuarioResponse.Email);
            Assert.True(usuarioResponse.ID > 0);
        }

        [Fact]
        public async Task GetUsuario_ExistingId_ReturnsUser()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = new Usuario
            {
                Nombre = "Usuario Existente",
                Email = "existente@test.com",
                FechaCreacion = DateTime.UtcNow
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var response = await _client.GetAsync($"/api/Usuario/{usuario.ID}");

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var usuarioResponse = JsonSerializer.Deserialize<UsuarioResponseDto>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(usuarioResponse);
            Assert.Equal(usuario.Nombre, usuarioResponse.Nombre);
            Assert.Equal(usuario.Email, usuarioResponse.Email);
        }

        [Fact]
        public async Task GetUsuario_NonExistingId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/Usuario/999");

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchUsuarios_WithFilters_ReturnsFilteredResults()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario1 = new Usuario
            {
                Nombre = "Jerónimo Bugao",
                Email = "jib@example.com",
                FechaCreacion = DateTime.UtcNow,
                Domicilio = new Domicilio
                {
                    Calle = "Av Montevideo",
                    Numero = "1092",
                    Provincia = "Córdoba",
                    Ciudad = "Capital",
                    FechaCreacion = DateTime.UtcNow
                }
            };

            var usuario2 = new Usuario
            {
                Nombre = "Rodrigo García",
                Email = "rodriwheels@example.com",
                FechaCreacion = DateTime.UtcNow,
                Domicilio = new Domicilio
                {
                    Calle = "Av 9 de Julio",
                    Numero = "2456",
                    Provincia = "Buenos Aires",
                    Ciudad = "CABA",
                    FechaCreacion = DateTime.UtcNow
                }
            };

            context.Usuarios.AddRange(usuario1, usuario2);
            await context.SaveChangesAsync();

            var searchDto = new SearchUsuarioDto
            {
                Provincia = "Córdoba"
            };

            var response = await _client.PostAsJsonAsync("/api/Usuario/search", searchDto);

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var usuarios = JsonSerializer.Deserialize<List<UsuarioResponseDto>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(usuarios);
            Assert.Single(usuarios);
            Assert.Equal("Jerónimo Bugao", usuarios.First().Nombre);
        }

        [Fact]
        public async Task UpdateUsuario_ValidData_ReturnsUpdatedUser()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = new Usuario
            {
                Nombre = "Nombre Original",
                Email = "original@test.com",
                FechaCreacion = DateTime.UtcNow
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var updateDto = new UpdateUsuarioDto
            {
                Nombre = "Nombre Actualizado",
                Email = "actualizado@test.com"
            };

            var response = await _client.PutAsJsonAsync($"/api/Usuario/{usuario.ID}", updateDto);

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var usuarioResponse = JsonSerializer.Deserialize<UsuarioResponseDto>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(usuarioResponse);
            Assert.Equal(updateDto.Nombre, usuarioResponse.Nombre);
            Assert.Equal(updateDto.Email, usuarioResponse.Email);
        }

        [Fact]
        public async Task DeleteUsuario_ExistingId_ReturnsNoContent()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = new Usuario
            {
                Nombre = "Usuario para eliminar",
                Email = "eliminar@test.com",
                FechaCreacion = DateTime.UtcNow
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var response = await _client.DeleteAsync($"/api/Usuario/{usuario.ID}");

            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync($"/api/Usuario/{usuario.ID}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}