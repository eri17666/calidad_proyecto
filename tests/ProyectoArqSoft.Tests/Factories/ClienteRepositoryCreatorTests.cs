using Microsoft.Extensions.Configuration;
using ProyectoArqSoft.FactoryCreators;
using ProyectoArqSoft.FactoryProducts;
using Xunit;
using System.Collections.Generic;

namespace ProyectoArqSoft.Tests.Factories
{
    public class ClienteRepositoryCreatorTests
    {
        [Fact]
        public void CreateRepo_DebeRetornarInstanciaValida_CuandoSeEjecutaLaFabrica()
        {
            // 1. Arrange: Configuramos el entorno (Cubre Nodos 1 y 2)
            var configData = new Dictionary<string, string> {
                {"ConnectionStrings:MySqlConnection", "Server=localhost;Database=test;"}
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
            var creator = new ClienteRepositoryCreator(config);

            // 2. Act: Ejecutamos el método (Cubre Nodos 3 y 4)
            var resultado = creator.CreateRepo();

            // 3. Assert: Verificamos el retorno (Cubre Nodo 5)
            Assert.NotNull(resultado);
            Assert.IsType<ClienteRepository>(resultado);
        }
    }
}
