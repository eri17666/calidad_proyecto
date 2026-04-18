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

        [Fact]
        public void ObtenerEstadisticas_DebeRetornarCeros_CuandoNoHayDatos()
        {
            var medicamentoRepo = new Mock<IMedicamentoRepository>();
            var clienteRepo = new Mock<IClienteRepository>();
            var bioquimicoRepo = new Mock<IBioquimicoRepository>();

            medicamentoRepo.Setup(x => x.Count()).Returns(0);
            clienteRepo.Setup(x => x.Count()).Returns(0);
            bioquimicoRepo.Setup(x => x.Count()).Returns(0);

            var service = new EstadisticasService(
                medicamentoRepo.Object,
                clienteRepo.Object,
                bioquimicoRepo.Object
            );

            var resultado = service.ObtenerEstadisticas();

            Assert.Equal(0, resultado.TotalMedicamentos);
            Assert.Equal(0, resultado.TotalClientes);
            Assert.Equal(0, resultado.TotalBioquimicos);
        }

        [Fact]
        public void ObtenerEstadisticas_DebeLlamarALosRepositorios()
        {
            var medicamentoRepo = new Mock<IMedicamentoRepository>();
            var clienteRepo = new Mock<IClienteRepository>();
            var bioquimicoRepo = new Mock<IBioquimicoRepository>();

            medicamentoRepo.Setup(x => x.Count()).Returns(1);
            clienteRepo.Setup(x => x.Count()).Returns(1);
            bioquimicoRepo.Setup(x => x.Count()).Returns(1);

            var service = new EstadisticasService(
                medicamentoRepo.Object,
                clienteRepo.Object,
                bioquimicoRepo.Object
            );

            service.ObtenerEstadisticas();

            medicamentoRepo.Verify(x => x.Count(), Times.Once);
            clienteRepo.Verify(x => x.Count(), Times.Once);
            bioquimicoRepo.Verify(x => x.Count(), Times.Once);
        }

        [Fact]
        public void ObtenerEstadisticas_DebeLanzarExcepcion_CuandoRepoFalla()
        {
            var medicamentoRepo = new Mock<IMedicamentoRepository>();
            var clienteRepo = new Mock<IClienteRepository>();
            var bioquimicoRepo = new Mock<IBioquimicoRepository>();

            medicamentoRepo.Setup(x => x.Count()).Throws(new Exception("DB error"));

            var service = new EstadisticasService(
                medicamentoRepo.Object,
                clienteRepo.Object,
                bioquimicoRepo.Object
            );

            Assert.Throws<Exception>(() => service.ObtenerEstadisticas());
        }

        [Fact]
        public void ObtenerEstadisticas_DebeLlamarRepositorios()
        {
            var medicamentoRepo = new Mock<IMedicamentoRepository>();
            var clienteRepo = new Mock<IClienteRepository>();
            var bioquimicoRepo = new Mock<IBioquimicoRepository>();

            medicamentoRepo.Setup(x => x.Count()).Returns(1);
            clienteRepo.Setup(x => x.Count()).Returns(1);
            bioquimicoRepo.Setup(x => x.Count()).Returns(1);

            var service = new EstadisticasService(
                medicamentoRepo.Object,
                clienteRepo.Object,
                bioquimicoRepo.Object
            );

            service.ObtenerEstadisticas();

            medicamentoRepo.Verify(x => x.Count(), Times.Once);
            clienteRepo.Verify(x => x.Count(), Times.Once);
            bioquimicoRepo.Verify(x => x.Count(), Times.Once);
        }

    }
}
