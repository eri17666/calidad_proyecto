using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Pages;
using System.Data;
using System.Text;
using Xunit;

namespace ProyectoArqSoft.Tests.Pages
{
    public class IndexModelTests
    {
        private readonly Mock<ILogger<IndexModel>> _mockLogger;
        private readonly Mock<IMedicamentoRepository> _mockRepository;
        private readonly Mock<ISession> _mockSession;
        private readonly IndexModel _model;
        private readonly IConfiguration _configuration;

        public IndexModelTests()
        {
            _mockLogger = new Mock<ILogger<IndexModel>>();
            _mockRepository = new Mock<IMedicamentoRepository>();
            _mockSession = new Mock<ISession>();

            // Usar configuración real sin conexión a BD
            _configuration = new ConfigurationBuilder().Build();

            _model = new IndexModel(_mockLogger.Object, _configuration, _mockRepository.Object);

            // Configurar HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _mockSession.Object;
            _model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
        }

        // ========== PRUEBAS DE OnGet ==========

        [Fact]
        public void OnGet_UsuarioEnSesion_AsignaUsuario()
        {
            // Arrange
            var usuarioBytes = Encoding.UTF8.GetBytes("juan.perez");
            byte[]? outBytes = usuarioBytes;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(true);

            _mockRepository.Setup(r => r.Count()).Returns(10);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal("juan.perez", _model.Usuario);
            Assert.Equal(10, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_SinUsuarioEnSesion_UsuarioEsNull()
        {
            // Arrange
            byte[]? outBytes = null;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(false);

            _mockRepository.Setup(r => r.Count()).Returns(5);

            // Act
            _model.OnGet();

            // Assert
            Assert.Null(_model.Usuario);
            Assert.Equal(5, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_TotalMedicamentosCero_AsignaCorrectamente()
        {
            // Arrange
            byte[]? outBytes = null;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(false);

            _mockRepository.Setup(r => r.Count()).Returns(0);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal(0, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_TotalMedicamentosGrande_AsignaCorrectamente()
        {
            // Arrange
            byte[]? outBytes = null;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(false);

            _mockRepository.Setup(r => r.Count()).Returns(9999);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal(9999, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_UsuarioConCaracteresEspeciales_AsignaCorrectamente()
        {
            // Arrange
            var usuarioBytes = Encoding.UTF8.GetBytes("maria.garcia@example.com");
            byte[]? outBytes = usuarioBytes;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(true);

            _mockRepository.Setup(r => r.Count()).Returns(3);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal("maria.garcia@example.com", _model.Usuario);
            Assert.Equal(3, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_MedicamentoDataTable_EsInicializado()
        {
            // Arrange
            byte[]? outBytes = null;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(false);

            _mockRepository.Setup(r => r.Count()).Returns(5);

            // Act
            _model.OnGet();

            // Assert
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_NoLanzaExcepcion_CuandoNoHayConexion()
        {
            // Arrange
            byte[]? outBytes = null;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(false);

            _mockRepository.Setup(r => r.Count()).Returns(5);

            // Act & Assert
            var exception = Record.Exception(() => _model.OnGet());

            // Assert
            Assert.Null(exception);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_UsuarioVacio_AsignaCadenaVacia()
        {
            // Arrange
            var usuarioBytes = Encoding.UTF8.GetBytes("");
            byte[]? outBytes = usuarioBytes;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(true);

            _mockRepository.Setup(r => r.Count()).Returns(1);

            // Act
            _model.OnGet();

            // Assert
            Assert.Equal("", _model.Usuario);
            Assert.Equal(1, _model.TotalMedicamentos);
        }

        [Fact]
        public void OnGet_UsuarioNull_NoLanzaExcepcion()
        {
            // Arrange
            byte[]? outBytes = null;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes))
                .Returns(false);

            _mockRepository.Setup(r => r.Count()).Returns(2);

            // Act
            var exception = Record.Exception(() => _model.OnGet());

            // Assert
            Assert.Null(exception);
            Assert.Null(_model.Usuario);
            Assert.Equal(2, _model.TotalMedicamentos);
        }
    }
}