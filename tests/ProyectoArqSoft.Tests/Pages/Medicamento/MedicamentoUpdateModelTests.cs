using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ProyectoArqSoft.Models;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Pages.Base;
using ProyectoArqSoft.Pages.EstadoPaginas;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;
using Xunit;
// Alias para evitar conflicto con el espacio de nombres
using MedicamentoModel = ProyectoArqSoft.Models.Medicamento;

namespace ProyectoArqSoft.Tests.Pages.Medicamento
{
    public class MedicamentoUpdateModelTests
    {
        private readonly Mock<IMedicamentoService> _mockService;
        private readonly MedicamentoUpdateModel _model;
        private readonly DefaultHttpContext _httpContext;
        private readonly TempDataDictionary _tempData;

        public MedicamentoUpdateModelTests()
        {
            _mockService = new Mock<IMedicamentoService>();
            _model = new MedicamentoUpdateModel(_mockService.Object);

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
        public void OnGet_NoRealizaOperaciones_RetornaSinErrores()
        {
            // Act
            var exception = Record.Exception(() => _model.OnGet());

            // Assert
            Assert.Null(exception);
        }

        // ========== PRUEBAS DE OnPostCargarMedicamentoParaEdicion ==========

        [Fact]
        public void OnPostCargarMedicamentoParaEdicion_MedicamentoExistente_CargaDatosYRetornaPage()
        {
            // Arrange
            var medicamento = new MedicamentoModel
            {
                Id = 1,
                Nombre = "Paracetamol",
                Presentacion = "Tableta",
                Clasificacion = "Analgésico",
                Concentracion = "500mg",
                Precio = 15.50m,
                Stock = 100
            };

            _mockService.Setup(s => s.ObtenerPorId(1))
                .Returns(medicamento);

            // Act
            var result = _model.OnPostCargarMedicamentoParaEdicion(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(1, _model.IdMedicamento);
            Assert.Equal("Paracetamol", _model.Nombre);
            Assert.Equal("Tableta", _model.Presentacion);
            Assert.Equal("Analgésico", _model.Clasificacion);
            Assert.Equal("500mg", _model.Concentracion);
            Assert.Equal(15.50m, _model.Precio);
            Assert.Equal(100, _model.Stock);
        }

        [Fact]
        public void OnPostCargarMedicamentoParaEdicion_MedicamentoNoExistente_RedirigeConError()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(999))
                .Returns((MedicamentoModel?)null);

            // Act
            var result = _model.OnPostCargarMedicamentoParaEdicion(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Medicamento", redirectResult.PageName);
            Assert.Equal("Medicamento no encontrado", redirectResult.RouteValues?["error"]);
        }

        [Fact]
        public void OnPostCargarMedicamentoParaEdicion_ConIdCero_RedirigeConError()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(0))
                .Returns((MedicamentoModel?)null);

            // Act
            var result = _model.OnPostCargarMedicamentoParaEdicion(0);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public void OnPostCargarMedicamentoParaEdicion_ConIdNegativo_RedirigeConError()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(-5))
                .Returns((MedicamentoModel?)null);

            // Act
            var result = _model.OnPostCargarMedicamentoParaEdicion(-5);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        // ========== PRUEBAS DE OnPostActualizarMedicamento ==========

        [Fact]
        public void OnPostActualizarMedicamento_DatosValidos_ActualizaYRedirige()
        {
            // Arrange
            _model.IdMedicamento = 1;
            _model.Nombre = "Paracetamol Actualizado";
            _model.Presentacion = "Tableta";
            _model.Clasificacion = "Analgésico";
            _model.Concentracion = "500mg";
            _model.Precio = 18.00m;
            _model.Stock = 80;

            _mockService.Setup(s => s.Actualizar(1, "Paracetamol Actualizado", "Tableta", "Analgésico", "500mg", 18.00m, 80))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostActualizarMedicamento();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Medicamento", redirectResult.PageName);
            Assert.Equal("Medicamento actualizado correctamente", redirectResult.RouteValues?["mensaje"]);
            _mockService.Verify(s => s.Actualizar(1, "Paracetamol Actualizado", "Tableta", "Analgésico", "500mg", 18.00m, 80), Times.Once);
        }

        [Fact]
        public void OnPostActualizarMedicamento_ValidacionFallida_RetornaPageConError()
        {
            // Arrange
            _model.IdMedicamento = 1;
            _model.Nombre = "Par";
            _model.Presentacion = "Tab";
            _model.Clasificacion = "Ana";
            _model.Concentracion = "500";
            _model.Precio = -10;
            _model.Stock = -5;

            var errorValidacion = Validacion.Fail("El nombre debe tener al menos 3 caracteres");
            _mockService.Setup(s => s.Actualizar(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostActualizarMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El nombre debe tener al menos 3 caracteres", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarMedicamento_MedicamentoNoExistente_RetornaError()
        {
            // Arrange
            _model.IdMedicamento = 999;
            _model.Nombre = "Ibuprofeno";
            _model.Presentacion = "Tableta";
            _model.Clasificacion = "Antiinflamatorio";
            _model.Concentracion = "400mg";
            _model.Precio = 20.00m;
            _model.Stock = 50;

            _mockService.Setup(s => s.Actualizar(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("El medicamento no existe"));

            // Act
            var result = _model.OnPostActualizarMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El medicamento no existe", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarMedicamento_NombreDuplicado_RetornaError()
        {
            // Arrange
            _model.IdMedicamento = 1;
            _model.Nombre = "Omeprazol";
            _model.Presentacion = "Cápsula";
            _model.Clasificacion = "Protector gástrico";
            _model.Concentracion = "20mg";
            _model.Precio = 25.00m;
            _model.Stock = 30;

            _mockService.Setup(s => s.Actualizar(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("Ya existe un medicamento con ese nombre"));

            // Act
            var result = _model.OnPostActualizarMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Ya existe un medicamento con ese nombre", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarMedicamento_PrecioCero_RetornaError()
        {
            // Arrange
            _model.IdMedicamento = 1;
            _model.Nombre = "Losartán";
            _model.Presentacion = "Tableta";
            _model.Clasificacion = "Antihipertensivo";
            _model.Concentracion = "50mg";
            _model.Precio = 0;
            _model.Stock = 40;

            _mockService.Setup(s => s.Actualizar(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("El precio debe ser mayor a cero"));

            // Act
            var result = _model.OnPostActualizarMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El precio debe ser mayor a cero", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarMedicamento_StockNegativo_RetornaError()
        {
            // Arrange
            _model.IdMedicamento = 1;
            _model.Nombre = "Amoxicilina";
            _model.Presentacion = "Cápsula";
            _model.Clasificacion = "Antibiótico";
            _model.Concentracion = "500mg";
            _model.Precio = 35.00m;
            _model.Stock = -10;

            _mockService.Setup(s => s.Actualizar(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("El stock no puede ser negativo"));

            // Act
            var result = _model.OnPostActualizarMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El stock no puede ser negativo", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarMedicamento_SinId_RetornaError()
        {
            // Arrange
            _model.IdMedicamento = 0;
            _model.Nombre = "Aspirina";
            _model.Presentacion = "Tableta";
            _model.Clasificacion = "Analgésico";
            _model.Concentracion = "100mg";
            _model.Precio = 12.00m;
            _model.Stock = 60;

            _mockService.Setup(s => s.Actualizar(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("ID de medicamento inválido"));

            // Act
            var result = _model.OnPostActualizarMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("ID de medicamento inválido", _model.Estado.MensajeError);
        }
    }
}