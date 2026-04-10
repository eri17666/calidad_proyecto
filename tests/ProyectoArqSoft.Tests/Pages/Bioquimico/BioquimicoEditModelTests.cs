using Moq;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Services;
using Xunit;

namespace ProyectoArqSoft.Tests.Pages
{
    public class BioquimicoEditModelTests
    {
        private readonly Mock<IBioquimicoService> _mockService;

        public BioquimicoEditModelTests()
        {
            _mockService = new Mock<IBioquimicoService>();
        }

        private BioquimicoEditModel CrearModelo()
        {
            return new BioquimicoEditModel(_mockService.Object);
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

            Assert.Equal(0, model.IdBioquimico); // int por defecto
            Assert.Equal(string.Empty, model.Nombres);
            Assert.Equal(string.Empty, model.ApellidoPaterno);
            Assert.Equal(string.Empty, model.ApellidoMaterno);
            Assert.Equal(string.Empty, model.Ci);
            Assert.Equal(string.Empty, model.CiExtencion);
            Assert.Equal(string.Empty, model.Telefono);
        }

        [Fact]
        public void Estado_SinError_NoDebeTenerMensaje()
        {
            var model = CrearModelo();

            Assert.True(string.IsNullOrEmpty(model.Estado.MensajeError));
        }

        [Fact]
        public void Estado_ConError_DeberiaAsignarse()
        {
            var model = CrearModelo();

            model.Estado.MensajeError = "Error test";

            Assert.Equal("Error test", model.Estado.MensajeError);
        }
    }
}