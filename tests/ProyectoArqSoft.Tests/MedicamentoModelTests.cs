using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;
using System.Data;

namespace ProyectoArqSoft.Tests
{
    public class MedicamentoModelTests
    {
        [Fact]
        public void OnGet_DebeCargarMedicamentos_CuandoFiltroEsValido()
        {
            var service = new Mock<IMedicamentoService>();
            var tabla = new DataTable();

            service.Setup(x => x.ObtenerTodos(It.IsAny<string>()))
                   .Returns(tabla);

            var model = new MedicamentoModel(service.Object);

            model.OnGet("", "", "");

            Assert.NotNull(model.MedicamentoDataTable);
        }

        [Fact]
        public void OnPostEliminarMedicamentoLogicamente_DebeRetornarPage_CuandoFalla()
        {
            var service = new Mock<IMedicamentoService>();

            service.Setup(x => x.EliminarLogicamente(1))
                   .Returns(Validacion.Fail("No se pudo eliminar"));

            var model = new MedicamentoModel(service.Object);

            var resultado = model.OnPostEliminarMedicamentoLogicamente(1);

            Assert.IsType<PageResult>(resultado);
            Assert.Equal("No se pudo eliminar", model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostEliminarMedicamentoLogicamente_DebeRedireccionar_CuandoSaleBien()
        {
            var service = new Mock<IMedicamentoService>();

            service.Setup(x => x.EliminarLogicamente(1))
                   .Returns(Validacion.Ok());

            var model = new MedicamentoModel(service.Object);

            var resultado = model.OnPostEliminarMedicamentoLogicamente(1);

            Assert.IsType<RedirectToPageResult>(resultado);
        }
    }
}
