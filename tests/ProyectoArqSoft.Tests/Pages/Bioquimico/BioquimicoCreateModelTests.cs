using Moq;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Services;
using Xunit;

namespace ProyectoArqSoft.Tests.Pages
{
    public class BioquimicoCreateModelTests
    {
        private readonly Mock<IBioquimicoService> _mockService;

        public BioquimicoCreateModelTests()
        {
            _mockService = new Mock<IBioquimicoService>();
        }

        private BioquimicoCreateModel CrearModelo()
        {
            return new BioquimicoCreateModel(_mockService.Object);
        }

        [Fact]
        public void Modelo_DeberiaInicializarCorrectamente()
        {
            var model = CrearModelo();

            Assert.NotNull(model);
            Assert.NotNull(model.Estado);
        }

        [Fact]
        public void Campos_DeberianInicializarVacios()
        {
            var model = CrearModelo();

            Assert.Equal(string.Empty, model.Nombres);
            Assert.Equal(string.Empty, model.ApellidoPaterno);
            Assert.Equal(string.Empty, model.ApellidoMaterno);
            Assert.Equal(string.Empty, model.Ci);
            Assert.Equal(string.Empty, model.CiExtencion);
            Assert.Equal(string.Empty, model.Telefono);
        }
    }
}