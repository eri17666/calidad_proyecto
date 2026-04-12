using Xunit;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests
{
    public class BioquimicoBusquedaValidacionTests
    {
        private readonly BioquimicoBusquedaValidacion _validador;

        public BioquimicoBusquedaValidacionTests()
        {
            _validador = new BioquimicoBusquedaValidacion();
        }

        [Fact]
        public void Validar_FiltroVacio_DebeRetornarOk()
        {
            var resultado = _validador.Validar("");

            Assert.False(resultado.IsFailure);
        }

        [Fact]
        public void Validar_NombreValido_DebeRetornarOk()
        {
            var resultado = _validador.Validar("Carlos");

            Assert.False(resultado.IsFailure);
        }

        [Fact]
        public void Validar_NumeroValido_DebeRetornarOk()
        {
            var resultado = _validador.Validar("123456");

            Assert.False(resultado.IsFailure);
        }

        [Fact]
        public void Validar_CIConExtension_DebeRetornarOk()
        {
            var resultado = _validador.Validar("123456-1A");

            Assert.False(resultado.IsFailure);
        }

        [Fact]
        public void Validar_CaracteresInvalidos_DebeRetornarFail()
        {
            var resultado = _validador.Validar("@@@###");

            Assert.True(resultado.IsFailure);
            Assert.Equal("Criterio inválido.", resultado.Error);
        }

        [Fact]
        public void Validar_TextoConSimbolos_DebeRetornarFail()
        {
            var resultado = _validador.Validar("Carlos@123");

            Assert.True(resultado.IsFailure);
        }
    }
}
