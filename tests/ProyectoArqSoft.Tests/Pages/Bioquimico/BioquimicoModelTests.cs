using Moq;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;
using System.Data;
using Xunit;

namespace ProyectoArqSoft.Tests.Pages
{
    public class BioquimicoModelTests
    {
        [Fact]
        public void Modelo_DeberiaInicializarCorrectamente()
        {
            // Arrange
            var mockService = new Mock<IBioquimicoService>();
            var mockValidacion = new Mock<IValidacion<string>>();

            var model = new BioquimicoModel(
                mockService.Object,
                mockValidacion.Object
            );

            // Assert
            Assert.NotNull(model);
        }

        [Fact]
        public void DataTable_Vacio_DeberiaTenerCeroFilas()
        {
            // Arrange
            var mockService = new Mock<IBioquimicoService>();
            var mockValidacion = new Mock<IValidacion<string>>();

            var model = new BioquimicoModel(
                mockService.Object,
                mockValidacion.Object
            );

            model.dtBioquimicos = new DataTable();

            // Assert
            Assert.Empty(model.dtBioquimicos.Rows);
        }
    }
}