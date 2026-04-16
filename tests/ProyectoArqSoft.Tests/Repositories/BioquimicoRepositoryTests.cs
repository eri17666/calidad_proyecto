using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ProyectoArqSoft.Models;
using ProyectoArqSoft.Repositories;
using Xunit;

namespace ProyectoArqSoft.Tests.Repositories
{
    public class BioquimicoRepositoryTests
    {
        private readonly BioquimicoRepository _repository;
        private readonly IConfiguration _configuration;

        public BioquimicoRepositoryTests()
        {
            // Cargamos el archivo appsettings.json para obtener la conexión real a Railway
            _configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _repository = new BioquimicoRepository(_configuration);
        }

        [Fact]
        public void Connection_DebeSerExitosa()
        {
            // Test de "Humo" para verificar que la IP de Railway no esté bloqueada
            var connectionString = _configuration.GetConnectionString("MySqlConnection");
            using var connection = new MySqlConnection(connectionString);

            connection.Open();

            Assert.Equal(ConnectionState.Open, connection.State);
        }

        [Fact]
        public void Insert_DebeRetornarUno_CuandoLosDatosSonValidos()
        {
            // Arrange
            var nuevoBioquimico = new Bioquimico
            {
                Nombres = "Test",
                ApellidoPaterno = "Caja",
                ApellidoMaterno = "Blanca",
                Ci = "0000000",
                CiExtencion = "SC",
                Telefono = "123456"
            };

            // Act
            int resultado = _repository.Insert(nuevoBioquimico);

            // Assert
            Assert.Equal(1, resultado);
        }

        [Fact]
        public void GetById_Camino1_DebeRetornarBioquimico_CuandoIdExiste()
        {
            // Arrange - Nota: Asegúrate de que exista al menos un registro con ID 1 en tu BD
            int idExistente = 1;

            // Act
            var resultado = _repository.GetById(idExistente);

            // Assert (Análisis de Caja Blanca: Camino reader.Read() == true)
            if (resultado != null)
            {
                Assert.Equal(idExistente, resultado.IdBioquimico);
                Assert.False(string.IsNullOrEmpty(resultado.Nombres));
            }
        }

        [Fact]
        public void GetById_Camino2_DebeRetornarNull_CuandoIdNoExiste()
        {
            // Arrange
            int idInexistente = 99999;

            // Act
            var resultado = _repository.GetById(idInexistente);

            // Assert (Análisis de Caja Blanca: Camino reader.Read() == false)
            Assert.Null(resultado);
        }

        [Fact]
        public void GetByDocumento_DebeFuncionar_ConDatosCorrectos()
        {
            // Arrange
            string ci = "0000000"; // Usamos el del test de inserción
            string ext = "SC";

            // Act
            var resultado = _repository.GetByDocumento(ci, ext);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(ci, resultado.Ci);
        }

        [Fact]
        public void Count_DebeRetornarNumeroTotalDeRegistros()
        {
            // Act
            int total = _repository.Count();

            // Assert
            Assert.True(total >= 0);
        }

        [Fact]
        public void GetAll_DebeRetornarDataTableConInformacion()
        {
            // Act
            DataTable dt = _repository.GetAll();

            // Assert
            Assert.NotNull(dt);
            // Si hay datos, debería tener columnas específicas
            Assert.Contains("nombres", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
        }

        [Fact]
        public void Update_DebeModificarRegistro_CuandoIdEsValido()
        {
            // Arrange
            var b = _repository.GetById(1); // Intentamos obtener el primero
            if (b != null)
            {
                b.Nombres = "Nombre Editado";

                // Act
                int filasAfectadas = _repository.Update(b);

                // Assert
                Assert.Equal(1, filasAfectadas);
            }
        }
    }
}