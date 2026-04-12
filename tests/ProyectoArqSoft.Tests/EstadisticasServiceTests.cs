using Moq;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Services;

namespace ProyectoArqSoft.Tests
{
    public class EstadisticasServiceTests
    {
        [Fact]
        public void ObtenerEstadisticas_DebeRetornarValoresCorrectos()
        {
            var medicamentoRepo = new Mock<IMedicamentoRepository>();
            var clienteRepo = new Mock<IClienteRepository>();
            var bioquimicoRepo = new Mock<IBioquimicoRepository>();

            medicamentoRepo.Setup(x => x.Count()).Returns(10);
            clienteRepo.Setup(x => x.Count()).Returns(5);
            bioquimicoRepo.Setup(x => x.Count()).Returns(3);

            var service = new EstadisticasService(
                medicamentoRepo.Object,
                clienteRepo.Object,
                bioquimicoRepo.Object
            );

            var resultado = service.ObtenerEstadisticas();

            Assert.Equal(10, resultado.TotalMedicamentos);
            Assert.Equal(5, resultado.TotalClientes);
            Assert.Equal(3, resultado.TotalBioquimicos);
        }
    }
}
