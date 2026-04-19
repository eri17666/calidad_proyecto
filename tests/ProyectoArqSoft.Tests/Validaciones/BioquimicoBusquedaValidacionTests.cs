using Xunit;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests.Validaciones
{
    public class BioquimicoBusquedaValidacionTests
    {
        private readonly BioquimicoBusquedaValidacion _validador;

        public BioquimicoBusquedaValidacionTests()
        {
            _validador = new BioquimicoBusquedaValidacion();
        }

        [Fact]
        public void Validar_FiltroNull_RetornaOk()
        {
            var resultado = _validador.Validar(null);
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroVacio_RetornaOk()
        {
            var resultado = _validador.Validar("");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroEspacios_RetornaOk()
        {
            var resultado = _validador.Validar("   ");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_NombreValido_RetornaOk()
        {
            var resultado = _validador.Validar("Juan Perez");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_NumeroValido_RetornaOk()
        {
            var resultado = _validador.Validar("12345");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_CIConExtension_RetornaOk()
        {
            var resultado = _validador.Validar("123456-1A");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_RangoNumerico_RetornaError()
        {
            var resultado = _validador.Validar("1-5");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void Validar_RangoConLetra_RetornaOk()
        {
            var resultado = _validador.Validar("1-5A");
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroLargo_RetornaError()
        {
            var resultado = _validador.Validar("texto que supera los veinte caracteres");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void Validar_CaracteresInvalidos_RetornaError()
        {
            var resultado = _validador.Validar("@@@###");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void Validar_TextoConSimbolos_RetornaError()
        {
            var resultado = _validador.Validar("Carlos@123");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void Validar_FiltroConArroba_RetornaError()
        {
            var resultado = _validador.Validar("juan@perez");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void Validar_FiltroConPunto_RetornaError()
        {
            var resultado = _validador.Validar("juan.perez");
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void ValidarLongitud_CadenaCorta_RetornaNull()
        {
            var resultado = _validador.ValidarLongitud("corto");
            Assert.Null(resultado);
        }

        [Fact]
        public void ValidarLongitud_CadenaLarga_RetornaError()
        {
            var resultado = _validador.ValidarLongitud(new string('x', 25));
            Assert.NotNull(resultado);
            Assert.True(resultado.IsFailure);
        }

        [Fact]
        public void ValidarFormato_Valido_RetornaNull()
        {
            var resultado = _validador.ValidarFormatoBusqueda("Juan Perez");
            Assert.Null(resultado);
        }

        [Fact]
        public void ValidarFormato_Invalido_RetornaError()
        {
            var resultado = _validador.ValidarFormatoBusqueda("juan@perez");
            Assert.NotNull(resultado);
            Assert.True(resultado.IsFailure);
        }
    }
}