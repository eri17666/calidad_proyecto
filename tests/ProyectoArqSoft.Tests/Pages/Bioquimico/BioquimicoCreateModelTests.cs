using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Pages.Base;
using ProyectoArqSoft.Pages.EstadoPaginas;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;
using Xunit;

namespace ProyectoArqSoft.Tests.Pages.Bioquimico
{
    public class BioquimicoCreateModelTests
    {
        private readonly Mock<IBioquimicoService> _mockService;
        private readonly BioquimicoCreateModel _model;

        public BioquimicoCreateModelTests()
        {
            _mockService = new Mock<IBioquimicoService>();
            _model = new BioquimicoCreateModel(_mockService.Object);

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

        // ========== PRUEBAS DE OnPostCrearBioquimico ==========

        [Fact]
        public void OnPostCrearBioquimico_DatosValidos_CreaYRedirige()
        {
            // Arrange
            _model.Nombres = "Juan Carlos";
            _model.ApellidoPaterno = "Perez";
            _model.ApellidoMaterno = "Gomez";
            _model.Ci = "1234567";
            _model.CiExtencion = "LP";
            _model.Telefono = "76543210";

            _mockService.Setup(s => s.Crear(It.IsAny<Models.Bioquimico>()))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostCrearBioquimico();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Bioquimico", redirectResult.PageName);
            _mockService.Verify(s => s.Crear(It.Is<Models.Bioquimico>(b =>
                b.Nombres == "Juan Carlos" &&
                b.ApellidoPaterno == "Perez" &&
                b.ApellidoMaterno == "Gomez" &&
                b.Ci == "1234567" &&
                b.CiExtencion == "LP" &&
                b.Telefono == "76543210")), Times.Once);
        }

        [Fact]
        public void OnPostCrearBioquimico_ValidacionFallida_RetornaPageConError()
        {
            // Arrange
            _model.Nombres = "Juan";
            _model.ApellidoPaterno = "P";
            _model.ApellidoMaterno = "G";
            _model.Ci = "123";
            _model.CiExtencion = "LP";
            _model.Telefono = "76543210";

            var errorValidacion = Validacion.Fail("El nombre debe tener entre 3 y 45 caracteres");
            _mockService.Setup(s => s.Crear(It.IsAny<Models.Bioquimico>()))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostCrearBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El nombre debe tener entre 3 y 45 caracteres", _model.Estado.MensajeError);
            _mockService.Verify(s => s.Crear(It.IsAny<Models.Bioquimico>()), Times.Once);
        }

        [Fact]
        public void OnPostCrearBioquimico_CamposVacios_RetornaError()
        {
            // Arrange
            _model.Nombres = "";
            _model.ApellidoPaterno = "";
            _model.ApellidoMaterno = "";
            _model.Ci = "";
            _model.CiExtencion = "";
            _model.Telefono = "";

            _mockService.Setup(s => s.Crear(It.IsAny<Models.Bioquimico>()))
                .Returns(Validacion.Fail("Todos los campos son obligatorios"));

            // Act
            var result = _model.OnPostCrearBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Todos los campos son obligatorios", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearBioquimico_CiDuplicada_RetornaError()
        {
            // Arrange
            _model.Nombres = "Maria";
            _model.ApellidoPaterno = "Lopez";
            _model.ApellidoMaterno = "Flores";
            _model.Ci = "9999999";
            _model.CiExtencion = "CB";
            _model.Telefono = "71234567";

            _mockService.Setup(s => s.Crear(It.IsAny<Models.Bioquimico>()))
                .Returns(Validacion.Fail("Ya existe un bioquímico con ese CI"));

            // Act
            var result = _model.OnPostCrearBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Ya existe un bioquímico con ese CI", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearBioquimico_TelefonoInvalido_RetornaError()
        {
            // Arrange
            _model.Nombres = "Carlos";
            _model.ApellidoPaterno = "Mendoza";
            _model.ApellidoMaterno = "Rios";
            _model.Ci = "5555555";
            _model.CiExtencion = "SC";
            _model.Telefono = "123";

            _mockService.Setup(s => s.Crear(It.IsAny<Models.Bioquimico>()))
                .Returns(Validacion.Fail("El teléfono debe tener 8 dígitos"));

            // Act
            var result = _model.OnPostCrearBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El teléfono debe tener 8 dígitos", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearBioquimico_CiExtensionInvalida_RetornaError()
        {
            // Arrange
            _model.Nombres = "Ana";
            _model.ApellidoPaterno = "Quispe";
            _model.ApellidoMaterno = "Mamani";
            _model.Ci = "7777777";
            _model.CiExtencion = "XX";
            _model.Telefono = "78901234";

            _mockService.Setup(s => s.Crear(It.IsAny<Models.Bioquimico>()))
                .Returns(Validacion.Fail("Extensión de CI no válida"));

            // Act
            var result = _model.OnPostCrearBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Extensión de CI no válida", _model.Estado.MensajeError);
        }
    }
}