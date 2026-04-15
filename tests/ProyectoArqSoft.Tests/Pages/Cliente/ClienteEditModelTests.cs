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
using ClienteModel = ProyectoArqSoft.Models.Cliente;

namespace ProyectoArqSoft.Tests.Pages.Cliente
{
    public class ClienteEditModelTests
    {
        private readonly Mock<IClienteService> _mockService;
        private readonly ClienteEditModel _model;
        private readonly DefaultHttpContext _httpContext;
        private readonly TempDataDictionary _tempData;

        public ClienteEditModelTests()
        {
            _mockService = new Mock<IClienteService>();
            _model = new ClienteEditModel(_mockService.Object);

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

        // ========== PRUEBAS DE OnPostCargarClienteParaEdicion ==========

        [Fact]
        public void OnPostCargarClienteParaEdicion_ClienteConsumidorFinal_CargaDatosYRetornaPage()
        {
            // Arrange
            var cliente = new ClienteModel
            {
                IdCliente = 1,
                Nit = "CF",
                RazonSocial = "Consumidor Final",
                CorreoElectronico = "cf@mail.com"
            };

            _mockService.Setup(s => s.ObtenerPorId(1))
                .Returns(cliente);

            // Act
            var result = _model.OnPostCargarClienteParaEdicion(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(1, _model.IdCliente);
            Assert.Equal("CF", _model.Nit);
            Assert.Equal("Consumidor Final", _model.RazonSocial);
            Assert.Equal("cf@mail.com", _model.CorreoElectronico);
            Assert.True(_model.EsConsumidorFinal);
        }

        [Fact]
        public void OnPostCargarClienteParaEdicion_ClienteEmpresa_CargaDatosYRetornaPage()
        {
            // Arrange
            var cliente = new ClienteModel
            {
                IdCliente = 2,
                Nit = "1234567890",
                RazonSocial = "Empresa SAC",
                CorreoElectronico = "empresa@mail.com"
            };

            _mockService.Setup(s => s.ObtenerPorId(2))
                .Returns(cliente);

            // Act
            var result = _model.OnPostCargarClienteParaEdicion(2);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(2, _model.IdCliente);
            Assert.Equal("1234567890", _model.Nit);
            Assert.Equal("Empresa SAC", _model.RazonSocial);
            Assert.Equal("empresa@mail.com", _model.CorreoElectronico);
            Assert.False(_model.EsConsumidorFinal);
        }

        [Fact]
        public void OnPostCargarClienteParaEdicion_ClienteNoExistente_RedirigeConError()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(999))
                .Returns((ClienteModel?)null);

            // Act
            var result = _model.OnPostCargarClienteParaEdicion(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Cliente", redirectResult.PageName);
            Assert.Equal("Cliente no encontrado", redirectResult.RouteValues?["error"]);
        }

        [Fact]
        public void OnPostCargarClienteParaEdicion_ConIdCero_RedirigeConError()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(0))
                .Returns((ClienteModel?)null);

            // Act
            var result = _model.OnPostCargarClienteParaEdicion(0);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public void OnPostCargarClienteParaEdicion_ConIdNegativo_RedirigeConError()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(-5))
                .Returns((ClienteModel?)null);

            // Act
            var result = _model.OnPostCargarClienteParaEdicion(-5);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        // ========== PRUEBAS DE OnPostActualizarCliente ==========

        [Fact]
        public void OnPostActualizarCliente_DatosValidos_ActualizaYRedirige()
        {
            // Arrange
            _model.IdCliente = 1;
            _model.EsConsumidorFinal = false;
            _model.Nit = "1234567890";
            _model.RazonSocial = "Empresa Actualizada";
            _model.CorreoElectronico = "actualizada@mail.com";

            _mockService.Setup(s => s.Actualizar(1, false, "1234567890", "Empresa Actualizada", "actualizada@mail.com"))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostActualizarCliente();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Cliente", redirectResult.PageName);
            Assert.Equal("Cliente actualizado correctamente", redirectResult.RouteValues?["mensaje"]);
            _mockService.Verify(s => s.Actualizar(1, false, "1234567890", "Empresa Actualizada", "actualizada@mail.com"), Times.Once);
        }

        [Fact]
        public void OnPostActualizarCliente_ConsumidorFinal_ActualizaYRedirige()
        {
            // Arrange
            _model.IdCliente = 2;
            _model.EsConsumidorFinal = true;
            _model.Nit = "CF";
            _model.RazonSocial = "Consumidor Final";
            _model.CorreoElectronico = "cf@mail.com";

            _mockService.Setup(s => s.Actualizar(2, true, "CF", "Consumidor Final", "cf@mail.com"))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostActualizarCliente();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            _mockService.Verify(s => s.Actualizar(2, true, "CF", "Consumidor Final", "cf@mail.com"), Times.Once);
        }

        [Fact]
        public void OnPostActualizarCliente_ValidacionFallida_RetornaPageConError()
        {
            // Arrange
            _model.IdCliente = 1;
            _model.EsConsumidorFinal = false;
            _model.Nit = "123";
            _model.RazonSocial = "Emp";
            _model.CorreoElectronico = "correo";

            var errorValidacion = Validacion.Fail("El NIT debe tener al menos 4 caracteres");
            _mockService.Setup(s => s.Actualizar(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostActualizarCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El NIT debe tener al menos 4 caracteres", _model.Estado.MensajeError);
            _mockService.Verify(s => s.Actualizar(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void OnPostActualizarCliente_ClienteNoExistente_RetornaError()
        {
            // Arrange
            _model.IdCliente = 999;
            _model.EsConsumidorFinal = false;
            _model.Nit = "1234567890";
            _model.RazonSocial = "Empresa Test";
            _model.CorreoElectronico = "test@mail.com";

            _mockService.Setup(s => s.Actualizar(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Validacion.Fail("El cliente no existe"));

            // Act
            var result = _model.OnPostActualizarCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El cliente no existe", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarCliente_NitDuplicado_RetornaError()
        {
            // Arrange
            _model.IdCliente = 1;
            _model.EsConsumidorFinal = false;
            _model.Nit = "9999999999";
            _model.RazonSocial = "Otra Empresa";
            _model.CorreoElectronico = "otra@mail.com";

            _mockService.Setup(s => s.Actualizar(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Validacion.Fail("Ya existe un cliente con ese NIT"));

            // Act
            var result = _model.OnPostActualizarCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Ya existe un cliente con ese NIT", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarCliente_CorreoInvalido_RetornaError()
        {
            // Arrange
            _model.IdCliente = 1;
            _model.EsConsumidorFinal = false;
            _model.Nit = "1234567890";
            _model.RazonSocial = "Empresa Test";
            _model.CorreoElectronico = "correo-invalido";

            _mockService.Setup(s => s.Actualizar(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Validacion.Fail("Formato de correo electrónico inválido"));

            // Act
            var result = _model.OnPostActualizarCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Formato de correo electrónico inválido", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarCliente_SinId_RetornaError()
        {
            // Arrange
            _model.IdCliente = 0;
            _model.EsConsumidorFinal = false;
            _model.Nit = "1234567890";
            _model.RazonSocial = "Empresa Test";
            _model.CorreoElectronico = "test@mail.com";

            _mockService.Setup(s => s.Actualizar(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Validacion.Fail("ID de cliente inválido"));

            // Act
            var result = _model.OnPostActualizarCliente();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("ID de cliente inválido", _model.Estado.MensajeError);
        }
    }
}