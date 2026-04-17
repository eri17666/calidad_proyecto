using Microsoft.Extensions.Configuration;
using ProyectoArqSoft.FactoryCreators; // <--- Verifica que este sea el namespace real
using ProyectoArqSoft.Repositories;
using ProyectoArqSoft.Models;
using System.Collections.Generic;
using Xunit;

namespace ProyectoArqSoft.Tests.Factories
{
    public class BioquimicoRepositoryCreatorTests
    {
        [Fact]
        public void CreateRepo_DebeRetornarBioquimicoRepository_AlEjecutarFabrica()
        {
            // Arrange: Configuración en memoria para evitar el NullReferenceException
            var configData = new Dictionary<string, string> {
                {"ConnectionStrings:MySqlConnection", "Server=localhost;Database=test;"}
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();

            var creator = new BioquimicoRepositoryCreator(config);

            // Act
            var resultado = creator.CreateRepo();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<BioquimicoRepository>(resultado);
        }
    }
}