using System.Data;
using Microsoft.Extensions.Configuration;
using ProyectoArqSoft.Models;
using ProyectoArqSoft.FactoryProducts;
using Xunit;

namespace ProyectoArqSoft.Tests.Repositories
{
    public class ClienteRepositoryTests
    {
        private readonly ClienteRepository _repository;
        private readonly IConfiguration _configuration;

        public ClienteRepositoryTests()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _repository = new ClienteRepository(_configuration);
        }

        [Fact]
        public void CoberturaTotal_ClienteRepository_Test()
        {
            // --- 1. PROBAR INSERT CON CORREO (Cubre ramas de Correo != null) ---
            var cliente = new Cliente
            {
                Nit = "8888888",
                RazonSocial = "Test Completo",
                CorreoElectronico = "test@correo.com" // Cubre rama True de línea 33
            };
            _repository.Insert(cliente);

            // --- 2. PROBAR GETBYID CON DATOS REALES (Cubre mapeo líneas 125-136) ---
            var temporal = _repository.ObtenerPorNit("8888888");
            Assert.NotNull(temporal);

            var clienteReal = _repository.GetById(temporal.IdCliente);
            Assert.NotNull(clienteReal); // Ahora las líneas 125-136 se pondrán verdes
            Assert.Equal("test@correo.com", clienteReal.CorreoElectronico);

            // --- 3. PROBAR OBTENERPORNIT QUE NO EXISTE (Cubre líneas 176-179) ---
            var noExiste = _repository.ObtenerPorNit("0000000");
            Assert.Null(noExiste); // Ahora las líneas 176-179 se pondrán verdes

            // --- 4. PROBAR GETALL SIN PARAMETROS (Cubre líneas 83-85) ---
            DataTable dtSinFiltro = _repository.GetAll();
            Assert.NotNull(dtSinFiltro); // Ahora las líneas 83-85 se pondrán verdes

            // --- 5. PROBAR UPDATE CON CORREO (Cubre rama de línea 59) ---
            clienteReal.RazonSocial = "Editado";
            int updateResult = _repository.Update(clienteReal);
            Assert.Equal(1, updateResult);

            // Limpieza
            _repository.Delete(clienteReal);
        }

        [Fact]
        public void GetById_NotFound_DebeRetornarNull()
        {
            // Esto ya lo tenías, cubre el 'return null' de la línea 141
            var result = _repository.GetById(-1);
            Assert.Null(result);
        }
    }
}