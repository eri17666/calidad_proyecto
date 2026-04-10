using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ProyectoArqSoft.Models;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests
{
    public class MedicamentoUpdateModelTests
    {
        [Fact]
        public void OnGet_DebeEjecutarseSinError()
        {
            var service = new Mock<IMedicamentoService>();

            var model = new MedicamentoUpdateModel(service.Object);

            model.OnGet();

            Assert.NotNull(model);
        }

        [Fact]
        public void OnPostCargarMedicamentoParaEdicion_DebeRedireccionar_CuandoNoExiste()
        {
            var service = new Mock<IMedicamentoService>();

            service.Setup(x => x.ObtenerPorId(1))
                   .Returns((Medicamento?)null);

            var model = new MedicamentoUpdateModel(service.Object);

            var resultado = model.OnPostCargarMedicamentoParaEdicion(1);

            var redirect = Assert.IsType<RedirectToPageResult>(resultado);
            Assert.Equal("Medicamento", redirect.PageName);
        }

        [Fact]
        public void OnPostCargarMedicamentoParaEdicion_DebeCargarDatos_CuandoExiste()
        {
            var service = new Mock<IMedicamentoService>();

            service.Setup(x => x.ObtenerPorId(1))
                   .Returns(new Medicamento
                   {
                       Id = 1,
                       Nombre = "Paracetamol",
                       Presentacion = "Tabletas",
                       Clasificacion = "Analgésico",
                       Concentracion = "500 mg",
                       Precio = 10.5m,
                       Stock = 25
                   });

            var model = new MedicamentoUpdateModel(service.Object);

            var resultado = model.OnPostCargarMedicamentoParaEdicion(1);

            Assert.IsType<PageResult>(resultado);
            Assert.Equal(1, model.IdMedicamento);
            Assert.Equal("Paracetamol", model.Nombre);
            Assert.Equal("Tabletas", model.Presentacion);
            Assert.Equal("Analgésico", model.Clasificacion);
            Assert.Equal("500 mg", model.Concentracion);
            Assert.Equal(10.5m, model.Precio);
            Assert.Equal(25, model.Stock);
        }

        [Fact]
        public void OnPostActualizarMedicamento_DebeRetornarPage_CuandoFalla()
        {
            var service = new Mock<IMedicamentoService>();

            service.Setup(x => x.Actualizar(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<int>()))
                .Returns(Validacion.Fail("Error al actualizar"));

            var model = new MedicamentoUpdateModel(service.Object)
            {
                IdMedicamento = 1,
                Nombre = "Ibuprofeno",
                Presentacion = "Tabletas",
                Clasificacion = "Antiinflamatorio",
                Concentracion = "400 mg",
                Precio = 12m,
                Stock = 15
            };

            var resultado = model.OnPostActualizarMedicamento();

            Assert.IsType<PageResult>(resultado);
            Assert.Equal("Error al actualizar", model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarMedicamento_DebeRedireccionar_CuandoSaleBien()
        {
            var service = new Mock<IMedicamentoService>();

            service.Setup(x => x.Actualizar(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<int>()))
                .Returns(Validacion.Ok());

            var model = new MedicamentoUpdateModel(service.Object)
            {
                IdMedicamento = 1,
                Nombre = "Ibuprofeno",
                Presentacion = "Tabletas",
                Clasificacion = "Antiinflamatorio",
                Concentracion = "400 mg",
                Precio = 12m,
                Stock = 15
            };

            var resultado = model.OnPostActualizarMedicamento();

            var redirect = Assert.IsType<RedirectToPageResult>(resultado);
            Assert.Equal("Medicamento", redirect.PageName);
        }
    }
}
