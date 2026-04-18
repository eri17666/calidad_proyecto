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
        public void CRUD_Operations_IntegrationTest()
        {
            // 1. Arrange: Crear un cliente de prueba
            var cliente = new Cliente
            {
                Nit = "9999999",
                RazonSocial = "Empresa de Prueba S.A.",
                CorreoElectronico = null // Probamos la rama de DBNull.Value (Línea 33)
            };

            // 2. Act: Insertar (Cubre Líneas 18-38)
            int insertResult = _repository.Insert(cliente);
            Assert.Equal(1, insertResult);

            // 3. Act: Obtener por NIT (Cubre Líneas 144-180)
            var clienteEncontrado = _repository.ObtenerPorNit("9999999");
            Assert.NotNull(clienteEncontrado);
            Assert.Equal("9999999", clienteEncontrado.Nit);
            Assert.Null(clienteEncontrado.UltimaActualizacion); // Rama de DBNull en fecha (Línea 172)

            // 4. Act: Update (Cubre Líneas 40-64)
            clienteEncontrado.RazonSocial = "Empresa Editada";
            int updateResult = _repository.Update(clienteEncontrado);
            Assert.Equal(1, updateResult);

            // 5. Act: Delete (Cubre Líneas 66-80)
            int deleteResult = _repository.Delete(clienteEncontrado);
            Assert.Equal(1, deleteResult);
        }

        [Fact]
        public void GetById_CaminoNotFound_DebeRetornarNull()
        {
            // Act
            var result = _repository.GetById(-1); // ID que no existe

            // Assert (Cubre la rama False del reader.Read - Línea 141)
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ConFiltro_DebeRetornarTabla()
        {
            // Act
            DataTable dt = _repository.GetAll("Empresa");

            // Assert (Cubre ConstruirQuery y FiltroSqlHelper - Líneas 88-106)
            Assert.NotNull(dt);
        }

        [Fact]
        public void Count_DebeRetornarValor()
        {
            // Act
            int count = _repository.Count();

            // Assert (Cubre Líneas 204-215)
            Assert.True(count >= 0);
        }
    }
}