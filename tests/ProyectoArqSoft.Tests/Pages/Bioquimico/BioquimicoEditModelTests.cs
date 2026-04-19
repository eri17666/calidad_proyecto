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
using BioquimicoModel = ProyectoArqSoft.Models.Bioquimico;

namespace ProyectoArqSoft.Tests.Pages.Bioquimico
{
    public class BioquimicoEditModelTests
    {
        private readonly Mock<IBioquimicoService> _mockService;
        private readonly BioquimicoEditModel _model;
        private readonly DefaultHttpContext _httpContext;
        private readonly TempDataDictionary _tempData;

        public BioquimicoEditModelTests()
        {
            _mockService = new Mock<IBioquimicoService>();
            _model = new BioquimicoEditModel(_mockService.Object);

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
        public void OnGet_NoRealizaOperaciones_RetornaSinErrores()
        {
            // Act
            var exception = Record.Exception(() => _model.OnGet());

            // Assert
            Assert.Null(exception);
        }

        // ========== PRUEBAS DE OnPostCargarBioquimicoParaEdicion ==========

        [Fact]
        public void OnPostCargarBioquimicoParaEdicion_BioquimicoExistente_CargaDatosYRetornaPage()
        {
            // Arrange
            var bioquimicoExistente = new BioquimicoModel
            {
                IdBioquimico = 1,
                Nombres = "Juan Carlos",
                ApellidoPaterno = "Perez",
                ApellidoMaterno = "Gomez",
                Ci = "1234567",
                CiExtencion = "LP",
                Telefono = "76543210"
            };

            _mockService.Setup(s => s.ObtenerPorId(1))
                .Returns(bioquimicoExistente);

            // Act
            var result = _model.OnPostCargarBioquimicoParaEdicion(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(1, _model.IdBioquimico);
            Assert.Equal("Juan Carlos", _model.Nombres);
            Assert.Equal("Perez", _model.ApellidoPaterno);
            Assert.Equal("Gomez", _model.ApellidoMaterno);
            Assert.Equal("1234567", _model.Ci);
            Assert.Equal("LP", _model.CiExtencion);
            Assert.Equal("76543210", _model.Telefono);
        }

        [Fact]
        public void OnPostCargarBioquimicoParaEdicion_BioquimicoNoExistente_RedirigeABioquimico()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(999))
                .Returns((BioquimicoModel?)null);

            // Act
            var result = _model.OnPostCargarBioquimicoParaEdicion(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Bioquimico", redirectResult.PageName);
        }

        [Fact]
        public void OnPostCargarBioquimicoParaEdicion_ConIdCero_RedirigeABioquimico()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(0))
                .Returns((BioquimicoModel?)null);

            // Act
            var result = _model.OnPostCargarBioquimicoParaEdicion(0);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public void OnPostCargarBioquimicoParaEdicion_ConIdNegativo_RedirigeABioquimico()
        {
            // Arrange
            _mockService.Setup(s => s.ObtenerPorId(-5))
                .Returns((BioquimicoModel?)null);

            // Act
            var result = _model.OnPostCargarBioquimicoParaEdicion(-5);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        // ========== PRUEBAS DE OnPostActualizarBioquimico ==========

        [Fact]
        public void OnPostActualizarBioquimico_DatosValidos_ActualizaYRedirige()
        {
            // Arrange
            _model.IdBioquimico = 1;
            _model.Nombres = "Juan Carlos Actualizado";
            _model.ApellidoPaterno = "Perez";
            _model.ApellidoMaterno = "Gomez";
            _model.Ci = "1234567";
            _model.CiExtencion = "LP";
            _model.Telefono = "76543210";

            _mockService.Setup(s => s.Actualizar(It.IsAny<BioquimicoModel>()))
                .Returns(Validacion.Ok());

            // Act
            var result = _model.OnPostActualizarBioquimico();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Bioquimico", redirectResult.PageName);
            Assert.Equal("Actualizado correctamente", _model.TempData["Mensaje"]);
            _mockService.Verify(s => s.Actualizar(It.Is<BioquimicoModel>(b =>
                b.IdBioquimico == 1 &&
                b.Nombres == "Juan Carlos Actualizado")), Times.Once);
        }

        [Fact]
        public void OnPostActualizarBioquimico_ValidacionFallida_RetornaPageConError()
        {
            // Arrange
            _model.IdBioquimico = 1;
            _model.Nombres = "J";
            _model.ApellidoPaterno = "P";
            _model.ApellidoMaterno = "G";
            _model.Ci = "123";
            _model.CiExtencion = "LP";
            _model.Telefono = "76543210";

            var errorValidacion = Validacion.Fail("El nombre debe tener entre 3 y 45 caracteres");
            _mockService.Setup(s => s.Actualizar(It.IsAny<BioquimicoModel>()))
                .Returns(errorValidacion);

            // Act
            var result = _model.OnPostActualizarBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El nombre debe tener entre 3 y 45 caracteres", _model.Estado.MensajeError);
            _mockService.Verify(s => s.Actualizar(It.IsAny<BioquimicoModel>()), Times.Once);
        }

        [Fact]
        public void OnPostActualizarBioquimico_BioquimicoNoExistente_RetornaError()
        {
            // Arrange
            _model.IdBioquimico = 999;
            _model.Nombres = "Carlos";
            _model.ApellidoPaterno = "Mendoza";
            _model.ApellidoMaterno = "Rios";
            _model.Ci = "5555555";
            _model.CiExtencion = "SC";
            _model.Telefono = "78901234";

            _mockService.Setup(s => s.Actualizar(It.IsAny<BioquimicoModel>()))
                .Returns(Validacion.Fail("El bioquímico no existe"));

            // Act
            var result = _model.OnPostActualizarBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("El bioquímico no existe", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarBioquimico_DatosParcialmenteInvalidos_RetornaError()
        {
            // Arrange
            _model.IdBioquimico = 1;
            _model.Nombres = "Ana";
            _model.ApellidoPaterno = "Quispe";
            _model.ApellidoMaterno = "Mamani";
            _model.Ci = "7777777";
            _model.CiExtencion = "XX";
            _model.Telefono = "78901234";

            _mockService.Setup(s => s.Actualizar(It.IsAny<BioquimicoModel>()))
                .Returns(Validacion.Fail("Extensión de CI no válida"));

            // Act
            var result = _model.OnPostActualizarBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("Extensión de CI no válida", _model.Estado.MensajeError);
        }

        [Fact]
        public void OnPostActualizarBioquimico_SinId_RetornaError()
        {
            // Arrange
            _model.IdBioquimico = 0;
            _model.Nombres = "Pedro";
            _model.ApellidoPaterno = "Torrez";
            _model.ApellidoMaterno = "Vargas";
            _model.Ci = "4444444";
            _model.CiExtencion = "OR";
            _model.Telefono = "70123456";

            _mockService.Setup(s => s.Actualizar(It.IsAny<BioquimicoModel>()))
                .Returns(Validacion.Fail("ID de bioquímico inválido"));

            // Act
            var result = _model.OnPostActualizarBioquimico();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal("ID de bioquímico inválido", _model.Estado.MensajeError);
        }
    }
}