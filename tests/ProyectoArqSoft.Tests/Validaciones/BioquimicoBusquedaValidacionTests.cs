using Xunit;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests.Validaciones
{
    public class BioquimicoBusquedaValidacionTests
    {
        private readonly BioquimicoBusquedaValidacion _validacion;

        public BioquimicoBusquedaValidacionTests()
        {
            _validacion = new BioquimicoBusquedaValidacion();
        }

        [Fact]
        public void Validar_FiltroNull_RetornaOk()
        {
            var resultado = _validacion.Validar(null);
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroVacio_RetornaOk()
        {
            var resultado = _validacion.Validar("");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroEspacios_RetornaOk()
        {
            var resultado = _validacion.Validar("   ");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroNombres_RetornaOk()
        {
            var resultado = _validacion.Validar("Juan Perez");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroSoloNumeros_RetornaOk()
        {
            var resultado = _validacion.Validar("12345");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroRangoNumerico_RetornaError()
        {
            var resultado = _validacion.Validar("1-5");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void Validar_FiltroRangoConLetra_RetornaOk()
        {
            var resultado = _validacion.Validar("1-5A");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroLargo_RetornaError()
        {
            var resultado = _validacion.Validar("texto que supera los veinte caracteres");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void Validar_FiltroConArroba_RetornaError()
        {
            var resultado = _validacion.Validar("juan@perez");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void Validar_FiltroConPunto_RetornaError()
        {
            var resultado = _validacion.Validar("juan.perez");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void ValidarLongitud_CadenaCorta_RetornaNull()
        {
            var resultado = _validacion.ValidarLongitud("corto");
            Assert.Null(resultado);
        }

        [Fact]
        public void ValidarLongitud_CadenaLarga_RetornaError()
        {
            var resultado = _validacion.ValidarLongitud(new string('x', 25));
            Assert.NotNull(resultado);
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void ValidarFormato_Valido_RetornaNull()
        {
            var resultado = _validacion.ValidarFormatoBusqueda("Juan Perez");
            Assert.Null(resultado);
        }

        [Fact]
        public void ValidarFormato_Invalido_RetornaError()
        {
            var resultado = _validacion.ValidarFormatoBusqueda("juan@perez");
            Assert.NotNull(resultado);
            Assert.True(resultado.IsFailure);
        }
    }
}