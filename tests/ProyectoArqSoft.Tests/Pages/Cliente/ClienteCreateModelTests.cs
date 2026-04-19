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

namespace ProyectoArqSoft.Tests.Pages.Cliente
{
    public class ClienteCreateModelTests
    {
        private readonly Mock<IClienteService> _mockService;
        private readonly ClienteCreateModel _model;

        public ClienteCreateModelTests()
        {
            _mockService = new Mock<IClienteService>();
            _model = new ClienteCreateModel(_mockService.Object);

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

        // ========== PRUEBAS DE OnPostCrearCliente ==========

        [Fact]
        public void OnPostCrearCliente_ConsumidorFinal_DatosValidos_CreaYRedirige()
        {
            // Arrange
            _model.EsConsumidorFinal = true;
            _model.Nit = "CF";
            _model.RazonSocial = "Consumidor Final";
            _model.CorreoElectronico = "cliente@mail.com";

            _mockService.Setup(s => s.Crear(true, "CF", "Consumidor Final", "cliente@mail.com"))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostCrearCliente();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Cliente", redirectResult.PageName);
            Assert.Equal("Cliente registrado correctamente", redirectResult.RouteValues?["mensaje"]);
            _mockService.Verify(s => s.Crear(true, "CF", "Consumidor Final", "cliente@mail.com"), Times.Once);
        }

        [Fact]
        public void OnPostCrearCliente_Empresa_DatosValidos_CreaYRedirige()
        {
            // Arrange
            _model.EsConsumidorFinal = false;
            _model.Nit = "1234567890";
            _model.RazonSocial = "Empresa SAC";
            _model.CorreoElectronico = "empresa@mail.com";

            _mockService.Setup(s => s.Crear(false, "1234567890", "Empresa SAC", "empresa@mail.com"))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostCrearCliente();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            _mockService.Verify(s => s.Crear(false, "1234567890", "Empresa SAC", "empresa@mail.com"), Times.Once);
        }

        [Fact]
        public void OnPostCrearCliente_ValidacionFallida_RetornaPageConError()
        {
            // Arrange
            _model.EsConsumidorFinal = false;
            _model.Nit = "123";
            _model.RazonSocial = "Emp";
            _model.CorreoElectronico = "correo";

            var errorValidacion = Validacion.Fail("El NIT debe tener al menos 4 caracteres");
            _mockService.Setup(s => s.Crear(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostCrearCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El NIT debe tener al menos 4 caracteres", _model.Estado.MensajeError);
            _mockService.Verify(s => s.Crear(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void OnPostCrearCliente_NitDuplicado_RetornaError()
        {
            // Arrange
            _model.EsConsumidorFinal = false;
            _model.Nit = "9999999999";
            _model.RazonSocial = "Otra Empresa";
            _model.CorreoElectronico = "otra@mail.com";

            _mockService.Setup(s => s.Crear(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Validacion.Fail("Ya existe un cliente con ese NIT"));

            // Act
            var result = _model.OnPostCrearCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Ya existe un cliente con ese NIT", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearCliente_CamposVacios_RetornaError()
        {
            // Arrange
            _model.EsConsumidorFinal = false;
            _model.Nit = "";
            _model.RazonSocial = "";
            _model.CorreoElectronico = "";

            _mockService.Setup(s => s.Crear(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Validacion.Fail("Todos los campos son obligatorios"));

            // Act
            var result = _model.OnPostCrearCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Todos los campos son obligatorios", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostCrearCliente_CorreoInvalido_RetornaError()
        {
            // Arrange
            _model.EsConsumidorFinal = false;
            _model.Nit = "1234567890";
            _model.RazonSocial = "Empresa Test";
            _model.CorreoElectronico = "correo-invalido";

            _mockService.Setup(s => s.Crear(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Validacion.Fail("Formato de correo electrónico inválido"));

            // Act
            var result = _model.OnPostCrearCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Formato de correo electrónico inválido", _model.Estado.MensajeError);
        }
    }
}