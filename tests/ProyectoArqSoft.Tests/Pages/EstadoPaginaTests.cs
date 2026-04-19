using Xunit;
using ProyectoArqSoft.Pages.EstadoPaginas;

namespace ProyectoArqSoft.Tests.Pages.EstadoPaginas
{
    public class EstadoPaginaTests
    {
        [Fact]
        public void Propiedades_DeberianInicializarseVacias()
        {
            // Arrange
            var estado = new EstadoPagina();

            // Assert
            Assert.NotNull(estado.FiltroActual);
            Assert.NotNull(estado.Mensaje);
            Assert.NotNull(estado.MensajeError);

            Assert.Equal(string.Empty, estado.FiltroActual);
            Assert.Equal(string.Empty, estado.Mensaje);
            Assert.Equal(string.Empty, estado.MensajeError);
        }

        [Fact]
        public void Propiedades_DeberianPermitirAsignacion()
        {
            // Arrange
            var estado = new EstadoPagina();

            // Act
            estado.FiltroActual = "test";
            estado.Mensaje = "ok";
            estado.MensajeError = "error";

            // Assert
            Assert.Equal("test", estado.FiltroActual);
            Assert.Equal("ok", estado.Mensaje);
            Assert.Equal("error", estado.MensajeError);
        }
    }
}