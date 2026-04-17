using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Pages;
using ProyectoArqSoft.Repositories;
using ProyectoArqSoft.Services;
using System.Data;
using System.Text;
using Xunit;

namespace ProyectoArqSoft.Tests.Pages
{
    public class IndexModelTests
    {
        private readonly Mock<ILogger<IndexModel>> _mockLogger;
        private readonly Mock<IMedicamentoRepository> _mockMedicamentoRepository;
        private readonly Mock<IClienteRepository> _mockClienteRepository;
        private readonly Mock<IBioquimicoRepository> _mockBioquimicoRepository;
        private readonly Mock<ISession> _mockSession;
        private readonly IndexModel _model;

        public IndexModelTests()
        {
            _mockLogger = new Mock<ILogger<IndexModel>>();
            _mockMedicamentoRepository = new Mock<IMedicamentoRepository>();
            _mockClienteRepository = new Mock<IClienteRepository>();
            _mockBioquimicoRepository = new Mock<IBioquimicoRepository>();
            _mockSession = new Mock<ISession>();

            _mockMedicamentoRepository.Setup(r => r.GetDestacados()).Returns(new DataTable());
            _mockClienteRepository.Setup(r => r.Count()).Returns(0);
            _mockBioquimicoRepository.Setup(r => r.Count()).Returns(0);

            _model = new IndexModel(
                _mockLogger.Object,
                _mockMedicamentoRepository.Object,
                _mockClienteRepository.Object,
                _mockBioquimicoRepository.Object);

            var httpContext = new DefaultHttpContext
            {
                Session = _mockSession.Object
            };

            _model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public void OnGet_UsuarioEnSesion_AsignaUsuario()
        {
            var usuarioBytes = Encoding.UTF8.GetBytes("juan.perez");
            byte[] outBytes = usuarioBytes;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(true);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(10);

            _model.OnGet();

            Assert.Equal("juan.perez", _model.Usuario);
            Assert.Equal(10, _model.TotalMedicamentos);
            Assert.Equal(0, _model.TotalClientes);
            Assert.Equal(0, _model.TotalBioquimicos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_SinUsuarioEnSesion_UsuarioEsNull()
        {
            byte[] outBytes = null!;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(false);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(5);

            _model.OnGet();

            Assert.Null(_model.Usuario);
            Assert.Equal(5, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_TotalMedicamentosCero_AsignaCorrectamente()
        {
            byte[] outBytes = null!;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(false);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(0);

            _model.OnGet();

            Assert.Equal(0, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_TotalMedicamentosGrande_AsignaCorrectamente()
        {
            byte[] outBytes = null!;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(false);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(9999);

            _model.OnGet();

            Assert.Equal(9999, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_UsuarioConCaracteresEspeciales_AsignaCorrectamente()
        {
            var usuarioBytes = Encoding.UTF8.GetBytes("maria.garcia@example.com");
            byte[] outBytes = usuarioBytes;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(true);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(3);

            _model.OnGet();

            Assert.Equal("maria.garcia@example.com", _model.Usuario);
            Assert.Equal(3, _model.TotalMedicamentos);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_MedicamentoDataTable_EsInicializado()
        {
            byte[] outBytes = null!;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(false);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(5);

            _model.OnGet();

            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_NoLanzaExcepcion_CuandoNoHayConexion()
        {
            byte[] outBytes = null!;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(false);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(5);

            var exception = Record.Exception(() => _model.OnGet());

            Assert.Null(exception);
            Assert.NotNull(_model.MedicamentoDataTable);
        }

        [Fact]
        public void OnGet_UsuarioVacio_AsignaCadenaVacia()
        {
            var usuarioBytes = Encoding.UTF8.GetBytes(string.Empty);
            byte[] outBytes = usuarioBytes;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(true);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(1);

            _model.OnGet();

            Assert.Equal(string.Empty, _model.Usuario);
            Assert.Equal(1, _model.TotalMedicamentos);
        }

        [Fact]
        public void OnGet_UsuarioNull_NoLanzaExcepcion()
        {
            byte[] outBytes = null!;
            _mockSession.Setup(s => s.TryGetValue("Usuario", out outBytes)).Returns(false);
            _mockMedicamentoRepository.Setup(r => r.Count()).Returns(2);

            var exception = Record.Exception(() => _model.OnGet());

            Assert.Null(exception);
            Assert.Null(_model.Usuario);
            Assert.Equal(2, _model.TotalMedicamentos);
        }
    }
}
