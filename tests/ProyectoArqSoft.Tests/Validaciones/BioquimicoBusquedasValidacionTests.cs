using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests
{
    public class BioquimicoBusquedasValidacionTests
    {
        [Fact]
        public void Validar_DebeRetornarOk_CuandoFiltroEsVacio()
        {
            var validador = new BioquimicoBusquedasValidacion();

            var resultado = validador.Validar("");

            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_DebeRetornarOk_CuandoFiltroEsNull()
        {
            var validador = new BioquimicoBusquedasValidacion();

            var resultado = validador.Validar(null!);

            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_DebeFallar_CuandoExcedeLongitud()
        {
            var validador = new BioquimicoBusquedasValidacion();

            string filtro = new string('a', 25); // más de 20

            var resultado = validador.Validar(filtro);

            Assert.True(resultado.IsFailure);
            Assert.Equal("El criterio es demasiado largo", resultado.Error);
        }

        [Fact]
        public void Validar_DebeRetornarOk_CuandoEsTextoValido()
        {
            var validador = new BioquimicoBusquedasValidacion();

            var resultado = validador.Validar("Carlos Perez");

            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_DebeRetornarOk_CuandoEsNumeroValido()
        {
            var validador = new BioquimicoBusquedasValidacion();

            var resultado = validador.Validar("123456");

            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_DebeRetornarOk_CuandoEsCIValido()
        {
            var validador = new BioquimicoBusquedasValidacion();

            var resultado = validador.Validar("1234567-1A");

            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_DebeFallar_CuandoFormatoEsInvalido()
        {
            var validador = new BioquimicoBusquedasValidacion();

            var resultado = validador.Validar("abc@123");

            Assert.True(resultado.IsFailure);
            Assert.Equal("Criterio de búsqueda inválido", resultado.Error);
        }
    }
}
