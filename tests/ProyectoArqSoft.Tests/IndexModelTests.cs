using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Repositories;
using ProyectoArqSoft.Services;
using System.Data;
using Xunit;

namespace ProyectoArqSoft.Tests
{
    public class IndexModelTests
    {
        [Fact]
        public void Constructor_DebeCrearInstancia()
        {
            var loggerMock = new Mock<ILogger<IndexModel>>();
            var medicamentoRepoMock = new Mock<IMedicamentoRepository>();
            var clienteRepoMock = new Mock<IClienteRepository>();
            var bioquimicoRepoMock = new Mock<IBioquimicoRepository>();

            var model = new IndexModel(
                loggerMock.Object,
                medicamentoRepoMock.Object,
                clienteRepoMock.Object,
                bioquimicoRepoMock.Object
            );

            Assert.NotNull(model);
        }

        [Fact]
        public void OnGet_DebeCargarTotalesCorrectamente()
        {
            var loggerMock = new Mock<ILogger<IndexModel>>();
            var medicamentoRepoMock = new Mock<IMedicamentoRepository>();
            var clienteRepoMock = new Mock<IClienteRepository>();
            var bioquimicoRepoMock = new Mock<IBioquimicoRepository>();

            medicamentoRepoMock.Setup(x => x.Count()).Returns(15);
            clienteRepoMock.Setup(x => x.Count()).Returns(20);
            bioquimicoRepoMock.Setup(x => x.Count()).Returns(5);

            DataTable tabla = new DataTable();
            tabla.Columns.Add("nombre");
            tabla.Rows.Add("Paracetamol");

            medicamentoRepoMock.Setup(x => x.GetDestacados())
                .Returns(tabla);

            var model = new IndexModel(
                loggerMock.Object,
                medicamentoRepoMock.Object,
                clienteRepoMock.Object,
                bioquimicoRepoMock.Object
            );

            // MOCK SESSION
            var sessionMock = new Mock<ISession>();

            byte[] usuarioBytes = System.Text.Encoding.UTF8.GetBytes("Jose");

            sessionMock
                .Setup(s => s.TryGetValue("Usuario", out usuarioBytes))
                .Returns(true);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            model.OnGet();

            Assert.Equal(15, model.TotalMedicamentos);
            Assert.Equal(20, model.TotalClientes);
            Assert.Equal(5, model.TotalBioquimicos);
            Assert.Single(model.MedicamentoDataTable.Rows);
        }

        [Fact]
        public void OnGet_DebeCargarUsuarioDesdeSession()
        {
            var loggerMock = new Mock<ILogger<IndexModel>>();
            var medicamentoRepoMock = new Mock<IMedicamentoRepository>();
            var clienteRepoMock = new Mock<IClienteRepository>();
            var bioquimicoRepoMock = new Mock<IBioquimicoRepository>();

            medicamentoRepoMock.Setup(x => x.Count()).Returns(1);
            clienteRepoMock.Setup(x => x.Count()).Returns(1);
            bioquimicoRepoMock.Setup(x => x.Count()).Returns(1);
            medicamentoRepoMock.Setup(x => x.GetDestacados()).Returns(new DataTable());

            var model = new IndexModel(
                loggerMock.Object,
                medicamentoRepoMock.Object,
                clienteRepoMock.Object,
                bioquimicoRepoMock.Object
            );

            var sessionMock = new Mock<ISession>();
            byte[] usuarioBytes = System.Text.Encoding.UTF8.GetBytes("Jose");

            sessionMock
                .Setup(s => s.TryGetValue("Usuario", out usuarioBytes))
                .Returns(true);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            model.OnGet();

            Assert.Equal("Jose", model.Usuario);
        }
        [Fact]
        public void OnGet_DebeFuncionar_SinUsuarioEnSession()
        {
            var loggerMock = new Mock<ILogger<IndexModel>>();
            var medicamentoRepoMock = new Mock<IMedicamentoRepository>();
            var clienteRepoMock = new Mock<IClienteRepository>();
            var bioquimicoRepoMock = new Mock<IBioquimicoRepository>();

            medicamentoRepoMock.Setup(x => x.Count()).Returns(1);
            clienteRepoMock.Setup(x => x.Count()).Returns(1);
            bioquimicoRepoMock.Setup(x => x.Count()).Returns(1);
            medicamentoRepoMock.Setup(x => x.GetDestacados()).Returns(new DataTable());

            var model = new IndexModel(
                loggerMock.Object,
                medicamentoRepoMock.Object,
                clienteRepoMock.Object,
                bioquimicoRepoMock.Object
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new Mock<ISession>().Object;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            model.OnGet();

            Assert.Null(model.Usuario);
        }


    }
}
