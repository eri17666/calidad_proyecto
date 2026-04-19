using Xunit;
using ProyectoArqSoft.Pages.Base;
using ProyectoArqSoft.Pages.EstadoPaginas;

namespace ProyectoArqSoft.Tests.Pages.Base
{
    // Clase concreta para poder testear la abstracta
    public class TestPageModel : BasePageModel
    {
    }

    public class BasePageModelTests
    {
        [Fact]
        public void Estado_DeberiaInicializarse_NoNulo()
        {
            var page = new TestPageModel();

            Assert.NotNull(page.Estado);
        }

        [Fact]
        public void Estado_DeberiaPermitirAsignacion()
        {
            var page = new TestPageModel();
            var nuevoEstado = new EstadoPagina();

            page.Estado = nuevoEstado;

            Assert.Equal(nuevoEstado, page.Estado);
        }
    }
}