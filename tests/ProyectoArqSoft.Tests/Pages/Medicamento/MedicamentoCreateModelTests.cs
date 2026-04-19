using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Pages.Base;
using ProyectoArqSoft.Pages.EstadoPaginas;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;
using Xunit;

namespace ProyectoArqSoft.Tests.Pages.Medicamento
{
    public class MedicamentoCreateModelTests
    {
        private readonly Mock<IMedicamentoService> _mockService;
        private readonly MedicamentoCreateModel _model;

        public MedicamentoCreateModelTests()
        {
            _mockService = new Mock<IMedicamentoService>();
            _model = new MedicamentoCreateModel(_mockService.Object);

            // Inicializar Estado
            _model.Estado = new EstadoPagina();
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

        // ========== PRUEBAS DE OnPostCrearMedicamento ==========

        [Fact]
        public void OnPostCrearMedicamento_DatosValidos_CreaYRedirige()
        {
            // Arrange
            _model.Nombre = "Paracetamol";
            _model.Presentacion = "Tableta";
            _model.Clasificacion = "Analgésico";
            _model.Concentracion = "500mg";
            _model.Precio = 15.50m;
            _model.Stock = 100;

            _mockService.Setup(s => s.Crear(
                "Paracetamol", "Tableta", "Analgésico", "500mg", 15.50m, 100))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostCrearMedicamento();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Medicamento", redirectResult.PageName);
            Assert.Equal("Medicamento registrado correctamente", redirectResult.RouteValues?["mensaje"]);
            _mockService.Verify(s => s.Crear(
                "Paracetamol", "Tableta", "Analgésico", "500mg", 15.50m, 100), Times.Once);
        }

        [Fact]
        public void OnPostCrearMedicamento_ValidacionFallida_RetornaPageConError()
        {
            // Arrange
            _model.Nombre = "Par";
            _model.Presentacion = "Tab";
            _model.Clasificacion = "Ana";
            _model.Concentracion = "500";
            _model.Precio = -10;
            _model.Stock = -5;

            var errorValidacion = Validacion.Fail("El nombre debe tener al menos 3 caracteres");
            _mockService.Setup(s => s.Crear(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostCrearMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El nombre debe tener al menos 3 caracteres", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearMedicamento_NombreDuplicado_RetornaError()
        {
            // Arrange
            _model.Nombre = "Ibuprofeno";
            _model.Presentacion = "Tableta";
            _model.Clasificacion = "Antiinflamatorio";
            _model.Concentracion = "400mg";
            _model.Precio = 20.00m;
            _model.Stock = 50;

            _mockService.Setup(s => s.Crear(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("Ya existe un medicamento con ese nombre"));

            // Act
            var result = _model.OnPostCrearMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Ya existe un medicamento con ese nombre", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearMedicamento_PrecioCero_RetornaError()
        {
            // Arrange
            _model.Nombre = "Omeprazol";
            _model.Presentacion = "Cápsula";
            _model.Clasificacion = "Protector gástrico";
            _model.Concentracion = "20mg";
            _model.Precio = 0;
            _model.Stock = 30;

            _mockService.Setup(s => s.Crear(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("El precio debe ser mayor a cero"));

            // Act
            var result = _model.OnPostCrearMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El precio debe ser mayor a cero", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearMedicamento_StockNegativo_RetornaError()
        {
            // Arrange
            _model.Nombre = "Losartán";
            _model.Presentacion = "Tableta";
            _model.Clasificacion = "Antihipertensivo";
            _model.Concentracion = "50mg";
            _model.Precio = 25.00m;
            _model.Stock = -10;

            _mockService.Setup(s => s.Crear(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("El stock no puede ser negativo"));

            // Act
            var result = _model.OnPostCrearMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El stock no puede ser negativo", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearMedicamento_CamposVacios_RetornaError()
        {
            // Arrange
            _model.Nombre = "";
            _model.Presentacion = "";
            _model.Clasificacion = "";
            _model.Concentracion = "";
            _model.Precio = 0;
            _model.Stock = 0;

            _mockService.Setup(s => s.Crear(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(Validacion.Fail("Todos los campos son obligatorios"));

            // Act
            var result = _model.OnPostCrearMedicamento();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Todos los campos son obligatorios", _model.Estado.MensajeError);
        }
    }
}