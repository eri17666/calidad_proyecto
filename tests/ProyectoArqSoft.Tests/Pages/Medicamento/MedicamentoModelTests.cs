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

namespace ProyectoArqSoft.Tests.Pages.Medicamento
{
    public class MedicamentoModelTests
    {
        private readonly Mock<IMedicamentoService> _mockService;
        private readonly MedicamentoModel _model;
        private readonly DefaultHttpContext _httpContext;
        private readonly TempDataDictionary _tempData;

        public MedicamentoModelTests()
        {
            _mockService = new Mock<IMedicamentoService>();
            _model = new MedicamentoModel(_mockService.Object);

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
        public void OnGet_FiltroNulo_ValidacionExitosa_CargaMedicamentos()
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
            Assert.Equal(dataTable, _model.MedicamentoDataTable);
            _mockService.Verify(s => s.ObtenerTodos(""), Times.Once);
        }

        [Fact]
        public void OnGet_FiltroValido_ValidacionExitosa_CargaMedicamentos()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Rows.Add(1);
            dataTable.Rows.Add(2);

            _mockService.Setup(s => s.ObtenerTodos("Paracetamol"))
                .Returns(dataTable);

            // Act
            _model.OnGet("Paracetamol", null, null);

            // Assert
            Assert.Equal(dataTable, _model.MedicamentoDataTable);
            _mockService.Verify(s => s.ObtenerTodos("Paracetamol"), Times.Once);
        }

        [Fact]
        public void OnGet_FiltroInvalido_NoCargaMedicamentos()
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

            _mockService.Setup(s => s.ObtenerTodos("Ibuprofeno"))
                .Returns(dataTableVacia);

            // Act
            _model.OnGet("Ibuprofeno", null, null);

            // Assert
            Assert.Equal(0, _model.MedicamentoDataTable.Rows.Count);
        }

        [Fact]
        public void OnGet_FiltroVacio_CargaTodosLosMedicamentos()
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
            Assert.Equal(3, _model.MedicamentoDataTable.Rows.Count);
            _mockService.Verify(s => s.ObtenerTodos(""), Times.Once);
        }

        // ========== PRUEBAS DE OnPostEliminarMedicamentoLogicamente ==========

        [Fact]
        public void OnPostEliminarMedicamentoLogicamente_EliminacionExitosa_Redirect()
        {
            // Arrange
            _mockService.Setup(s => s.EliminarLogicamente(1))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostEliminarMedicamentoLogicamente(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            _mockService.Verify(s => s.EliminarLogicamente(1), Times.Once);
        }

        [Fact]
        public void OnPostEliminarMedicamentoLogicamente_EliminacionFallida_RetornaPageConError()
        {
            // Arrange
            var errorValidacion = Validacion.Fail("No se puede eliminar el medicamento");
            _mockService.Setup(s => s.EliminarLogicamente(2))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostEliminarMedicamentoLogicamente(2);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("No se puede eliminar el medicamento", _model.Estado.MensajeError);
            _mockService.Verify(s => s.EliminarLogicamente(2), Times.Once);
        }

        [Fact]
        public void OnPostEliminarMedicamentoLogicamente_ConIdCero_RetornaError()
        {
            // Arrange
            _mockService.Setup(s => s.EliminarLogicamente(0))
                .Returns(Validacion.Fail("ID inválido"));

            // Act
            var result = _model.OnPostEliminarMedicamentoLogicamente(0);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("ID inválido", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostEliminarMedicamentoLogicamente_ConIdNegativo_RetornaError()
        {
            // Arrange
            _mockService.Setup(s => s.EliminarLogicamente(-5))
                .Returns(Validacion.Fail("ID no puede ser negativo"));

            // Act
            var result = _model.OnPostEliminarMedicamentoLogicamente(-5);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("ID no puede ser negativo", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostEliminarMedicamentoLogicamente_ConIdInexistente_RetornaError()
        {
            // Arrange
            _mockService.Setup(s => s.EliminarLogicamente(999))
                .Returns(Validacion.Fail("Medicamento no encontrado"));

            // Act
            var result = _model.OnPostEliminarMedicamentoLogicamente(999);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Medicamento no encontrado", _model.Estado.MensajeError);
        }
    }
}