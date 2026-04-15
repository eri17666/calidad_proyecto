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

namespace ProyectoArqSoft.Tests.Pages.Bioquimico
{
    public class BioquimicoModelTests
    {
        private readonly Mock<IBioquimicoService> _mockService;
        private readonly Mock<IValidacion<string>> _mockValidator;
        private readonly BioquimicoModel _model;
        private readonly DefaultHttpContext _httpContext;
        private readonly TempDataDictionary _tempData;

        public BioquimicoModelTests()
        {
            _mockService = new Mock<IBioquimicoService>();
            _mockValidator = new Mock<IValidacion<string>>();
            _model = new BioquimicoModel(_mockService.Object, _mockValidator.Object);

            // Inicializar Estado
            _model.Estado = new EstadoPagina();

            // Inicializar TempData para evitar NullReferenceException
            _httpContext = new DefaultHttpContext();
            _tempData = new TempDataDictionary(_httpContext, Mock.Of<ITempDataProvider>());
            _model.TempData = _tempData;

            // Inicializar PageContext para TempData
            _model.PageContext = new PageContext
            {
                HttpContext = _httpContext
            };
        }

        // ========== PRUEBAS DE OnGet ==========

        [Fact]
        public void OnGet_FiltroNulo_ValidacionExitosa_CargaBioquimicos()
        {
            // Arrange
            _model.Filtro = null;
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Rows.Add(1);

            _mockValidator.Setup(v => v.Validar(""))
                .Returns(Validacion.Ok());
            _mockService.Setup(s => s.ObtenerTodos(""))
                .Returns(dataTable);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal(dataTable, _model.dtBioquimicos);
            _mockService.Verify(s => s.ObtenerTodos(""), Times.Once);
        }

        [Fact]
        public void OnGet_FiltroVacio_ValidacionExitosa_CargaBioquimicos()
        {
            // Arrange
            _model.Filtro = "";
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Rows.Add(1);

            _mockValidator.Setup(v => v.Validar(""))
                .Returns(Validacion.Ok());
            _mockService.Setup(s => s.ObtenerTodos(""))
                .Returns(dataTable);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal(dataTable, _model.dtBioquimicos);
        }

        [Fact]
        public void OnGet_FiltroValido_ValidacionExitosa_CargaBioquimicos()
        {
            // Arrange
            _model.Filtro = "Juan";
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Rows.Add(1);
            dataTable.Rows.Add(2);

            _mockValidator.Setup(v => v.Validar("Juan"))
                .Returns(Validacion.Ok());
            _mockService.Setup(s => s.ObtenerTodos("Juan"))
                .Returns(dataTable);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal(dataTable, _model.dtBioquimicos);
        }

        [Fact]
        public void OnGet_ValidacionFallida_NoCargaBioquimicos_AsignaError()
        {
            // Arrange
            _model.Filtro = "invalido@";
            var errorValidacion = Validacion.Fail("Criterio de búsqueda inválido");

            _mockValidator.Setup(v => v.Validar("invalido@"))
                .Returns(errorValidacion);

            // Act
            _model.OnGet();

            // Assert
            Assert.NotNull(_model.dtBioquimicos);
            Assert.Equal(0, _model.dtBioquimicos.Rows.Count);
            Assert.Equal("Criterio de búsqueda inválido", _model.Estado.MensajeError);
            _mockService.Verify(s => s.ObtenerTodos(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void OnGet_SinResultadosConFiltro_AsignaMensajeNoEncontrado()
        {
            // Arrange
            _model.Filtro = "Perez";
            var dataTableVacia = new DataTable();
            dataTableVacia.Columns.Add("Id");

            _mockValidator.Setup(v => v.Validar("Perez"))
                .Returns(Validacion.Ok());
            _mockService.Setup(s => s.ObtenerTodos("Perez"))
                .Returns(dataTableVacia);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal(0, _model.dtBioquimicos.Rows.Count);
            Assert.Equal("No se encontraron resultados para: Perez", _model.Estado.Mensaje);
        }

        [Fact]
        public void OnGet_SinResultadosSinFiltro_NoAsignaMensaje()
        {
            // Arrange
            _model.Filtro = "";
            var dataTableVacia = new DataTable();
            dataTableVacia.Columns.Add("Id");

            _mockValidator.Setup(v => v.Validar(""))
                .Returns(Validacion.Ok());
            _mockService.Setup(s => s.ObtenerTodos(""))
                .Returns(dataTableVacia);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal(0, _model.dtBioquimicos.Rows.Count);
        }

        // ========== PRUEBAS DE OnPostEliminar ==========

        [Fact]
        public void OnPostEliminar_EliminacionExitosa_RedirectConMensaje()
        {
            // Arrange
            _mockService.Setup(s => s.Eliminar(1))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostEliminar(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Bioquímico eliminado correctamente.", _model.TempData["Mensaje"]);
            Assert.Null(_model.TempData["Error"]);
            _mockService.Verify(s => s.Eliminar(1), Times.Once);
        }

        [Fact]
        public void OnPostEliminar_EliminacionFallida_RedirectConError()
        {
            // Arrange
            var errorValidacion = Validacion.Fail("No se puede eliminar porque tiene registros asociados");
            _mockService.Setup(s => s.Eliminar(2))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostEliminar(2);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("No se puede eliminar porque tiene registros asociados", _model.TempData["Error"]);
            Assert.Null(_model.TempData["Mensaje"]);
            _mockService.Verify(s => s.Eliminar(2), Times.Once);
        }

        [Fact]
        public void OnPostEliminar_ConIdCero_RetornaError()
        {
            // Arrange
            _mockService.Setup(s => s.Eliminar(0))
                .Returns(Validacion.Fail("ID inválido"));

            // Act
            var result = _model.OnPostEliminar(0);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("ID inválido", _model.TempData["Error"]);
        }

        [Fact]
        public void OnPostEliminar_ConIdNegativo_RetornaError()
        {
            // Arrange
            _mockService.Setup(s => s.Eliminar(-5))
                .Returns(Validacion.Fail("ID no puede ser negativo"));

            // Act
            var result = _model.OnPostEliminar(-5);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("ID no puede ser negativo", _model.TempData["Error"]);
        }
    }
}