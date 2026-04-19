using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Pages.Base;
using ProyectoArqSoft.Pages.EstadoPaginas;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;
using System.Data;
using Xunit;

namespace ProyectoArqSoft.Tests.Pages.Cliente
{
    public class ClienteModelTests
    {
        private readonly Mock<IClienteService> _mockService;
        private readonly ClienteModel _model;
        private readonly DefaultHttpContext _httpContext;
        private readonly TempDataDictionary _tempData;

        public ClienteModelTests()
        {
            _mockService = new Mock<IClienteService>();
            _model = new ClienteModel(_mockService.Object);

            // Inicializar Estado
            _model.Estado = new EstadoPagina();

            // Inicializar TempData
            _httpContext = new DefaultHttpContext();
            _tempData = new TempDataDictionary(_httpContext, Mock.Of<ITempDataProvider>());
            _model.TempData = _tempData;

            // Inicializar PageContext
            _model.PageContext = new PageContext
            {
                HttpContext = _httpContext
            };
        }

        // ========== PRUEBAS DE OnGet ==========

        [Fact]
        public void OnGet_FiltroNulo_ValidacionExitosa_CargaClientes()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Rows.Add(1);

            _mockService.Setup(s => s.ObtenerTodos(""))
                .Returns(dataTable);

            // Act
            _model.OnGet(null, null, null);

            // Assert
            Assert.Equal(dataTable, _model.ClienteDataTable);
            _mockService.Verify(s => s.ObtenerTodos(""), Times.Once);
        }

        [Fact]
        public void OnGet_FiltroValido_ValidacionExitosa_CargaClientes()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Rows.Add(1);
            dataTable.Rows.Add(2);

            _mockService.Setup(s => s.ObtenerTodos("Juan"))
                .Returns(dataTable);

            // Act
            _model.OnGet("Juan", null, null);

            // Assert
            Assert.Equal(dataTable, _model.ClienteDataTable);
            _mockService.Verify(s => s.ObtenerTodos("Juan"), Times.Once);
        }

        [Fact]
        public void OnGet_FiltroInvalido_NoCargaClientes()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerTodos(It.IsAny<string>()))
                .Returns(new DataTable());

            // Act
            _model.OnGet("filtro@invalido", null, null);

            // Assert
            _mockService.Verify(s => s.ObtenerTodos(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void OnGet_ConMensaje_AsignaCorrectamenteMensaje()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Rows.Add(1);

            _mockService.Setup(s => s.ObtenerTodos(""))
                .Returns(dataTable);

            // Act
            _model.OnGet("", "Operación exitosa", null);

            // Assert
            Assert.Equal("Operación exitosa", _model.Estado.Mensaje);
        }

        [Fact]
        public void OnGet_SinResultados_DataTableVacia()
        {
            // Arrange
            var dataTableVacia = new DataTable();
            dataTableVacia.Columns.Add("Id");

            _mockService.Setup(s => s.ObtenerTodos("Perez"))
                .Returns(dataTableVacia);

            // Act
            _model.OnGet("Perez", null, null);

            // Assert
            Assert.Equal(0, _model.ClienteDataTable.Rows.Count);
        }

        [Fact]
        public void OnGet_FiltroVacio_CargaTodosLosClientes()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Rows.Add(1);
            dataTable.Rows.Add(2);
            dataTable.Rows.Add(3);

            _mockService.Setup(s => s.ObtenerTodos(""))
                .Returns(dataTable);

            // Act
            _model.OnGet("", null, null);

            // Assert
            Assert.Equal(3, _model.ClienteDataTable.Rows.Count);
            _mockService.Verify(s => s.ObtenerTodos(""), Times.Once);
        }

        // ========== PRUEBAS DE OnPostEliminarClienteLogicamente ==========

        [Fact]
        public void OnPostEliminarClienteLogicamente_EliminacionExitosa_RedirectConMensaje()
        {
            // Arrange
            _mockService.Setup(s => s.Eliminar(1))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostEliminarClienteLogicamente(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Cliente", redirectResult.PageName);
            Assert.Equal("Cliente eliminado correctamente", redirectResult.RouteValues?["mensaje"]);
            _mockService.Verify(s => s.Eliminar(1), Times.Once);
        }

        [Fact]
        public void OnPostEliminarClienteLogicamente_EliminacionFallida_RetornaPageConError()
        {
            // Arrange
            var errorValidacion = Validacion.Fail("No se puede eliminar el cliente");
            _mockService.Setup(s => s.Eliminar(2))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostEliminarClienteLogicamente(2);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("No se puede eliminar el cliente", _model.Estado.MensajeError);
            _mockService.Verify(s => s.Eliminar(2), Times.Once);
        }

        [Fact]
        public void OnPostEliminarClienteLogicamente_ConIdCero_RetornaError()
        {
            // Arrange
            _mockService.Setup(s => s.Eliminar(0))
                .Returns(Validacion.Fail("ID inválido"));

            // Act
            var result = _model.OnPostEliminarClienteLogicamente(0);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("ID inválido", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostEliminarClienteLogicamente_ConIdNegativo_RetornaError()
        {
            // Arrange
            _mockService.Setup(s => s.Eliminar(-5))
                .Returns(Validacion.Fail("ID no puede ser negativo"));

            // Act
            var result = _model.OnPostEliminarClienteLogicamente(-5);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("ID no puede ser negativo", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostEliminarClienteLogicamente_ConIdGrande_RetornaError()
        {
            // Arrange
            _mockService.Setup(s => s.Eliminar(99999))
                .Returns(Validacion.Fail("Cliente no encontrado"));

            // Act
            var result = _model.OnPostEliminarClienteLogicamente(99999);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Cliente no encontrado", _model.Estado.MensajeError);
        }
    }
}