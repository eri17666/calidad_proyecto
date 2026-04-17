using Xunit;
using ProyectoArqSoft.Validaciones;
using System.Text.RegularExpressions;

namespace ProyectoArqSoft.Tests.Validaciones
{
    public class BioquimicoBusquedaValidacionTests
    {
        private readonly BioquimicoBusquedaValidacion _validador;

        public BioquimicoBusquedaValidacionTests()
        {
            _validador = new BioquimicoBusquedaValidacion();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validar_FiltroNuloOVacio_RetornaOk(string filtro)
        {
            // Act
            var resultado = _validador.Validar(filtro);

            // Assert (Cubre la primera rama del método Validar)
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Validar_FiltroExcedeLongitud_RetornaFail()
        {
            // Arrange
            string filtroLargo = "Este texto tiene mas de veinte caracteres";

            // Act
            var resultado = _validador.Validar(filtroLargo);

            // Assert (Cubre ValidarLongitud dentro de Validar)
            Assert.True(resultado.IsFailure);
            Assert.Equal("El criterio excede los 20 caracteres", resultado.Error);
        }

        [Theory]
        [InlineData("Juan@123")]
        [InlineData("123#")]
        [InlineData("!!!")]
        public void Validar_FormatoInvalido_RetornaFail(string filtro)
        {
            // Act
            var resultado = _validador.Validar(filtro);

            // Assert (Cubre ValidarFormatoBusqueda dentro de Validar)
            Assert.True(resultado.IsFailure);
            Assert.Equal("Criterio de búsqueda inválido. Use letras, números o rango (ej: 1-5A)", resultado.Error);
        }

        [Theory]
        [InlineData("Juan Perez")]
        [InlineData("1234567")]
        [InlineData("123456-1A")]
        public void Validar_CriteriosValidos_RetornaOk(string filtro)
        {
            // Act
            var resultado = _validador.Validar(filtro);

            // Assert (Cubre el final del método Validar)
            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void ValidarLongitud_DentroDelLimite_RetornaNull()
        {
            // Act
            var resultado = _validador.ValidarLongitud("Corto");

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void ValidarFormato_RegexValido_RetornaNull()
        {
            // Act
            var resultado = _validador.ValidarFormatoBusqueda("12345");

            // Assert
            Assert.Null(resultado);
        }
    }
}