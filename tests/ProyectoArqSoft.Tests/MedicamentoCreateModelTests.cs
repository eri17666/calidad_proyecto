using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests
{
    public class MedicamentoCreateModelTests
    {
        [Fact]
        public void OnGet_DebeEjecutarseSinError()
        {
            var service = new Mock<IMedicamentoService>();

            var model = new MedicamentoCreateModel(service.Object);

            model.OnGet();

            Assert.NotNull(model);
        }

        [Fact]
        public void OnPostCrearMedicamento_DebeRetornarPage_CuandoFalla()
        {
            var service = new Mock<IMedicamentoService>();

            service.Setup(x => x.Crear(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<int>()))
                .Returns(Validacion.Fail("Error al registrar"));

            var model = new MedicamentoCreateModel(service.Object)
            {
                Nombre = "Paracetamol",
                Presentacion = "Tabletas",
                Clasificacion = "Analgésico",
                Concentracion = "500 mg",
                Precio = 10.5m,
                Stock = 50
            };

            var resultado = model.OnPostCrearMedicamento();

            Assert.IsType<PageResult>(resultado);
            Assert.Equal("Error al registrar", model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearMedicamento_DebeRedireccionar_CuandoSaleBien()
        {
            var service = new Mock<IMedicamentoService>();

            service.Setup(x => x.Crear(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<int>()))
                .Returns(Validacion.Ok());

            var model = new MedicamentoCreateModel(service.Object)
            {
                Nombre = "Ibuprofeno",
                Presentacion = "Tabletas",
                Clasificacion = "Antiinflamatorio",
                Concentracion = "400 mg",
                Precio = 15m,
                Stock = 30
            };

            var resultado = model.OnPostCrearMedicamento();

            var redirect = Assert.IsType<RedirectToPageResult>(resultado);
            Assert.Equal("Medicamento", redirect.PageName);
        }
    }
}
