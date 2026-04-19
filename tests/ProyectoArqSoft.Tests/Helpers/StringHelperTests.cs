using ProyectoArqSoft.Helpers;
using Xunit;

namespace ProyectoArqSoft.Tests.Helpers
{
    public class StringHelperTests
    {
        [Fact]
        public void Limpiar_DebeRetornarVacio_CuandoTextoEsNull()
        {
            string resultado = StringHelper.Limpiar(null);

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void Limpiar_DebeQuitarEspaciosExtremos()
        {
            string resultado = StringHelper.Limpiar("  texto de prueba  ");

            Assert.Equal("texto de prueba", resultado);
        }

        [Fact]
        public void LimpiarEspacios_DebeRetornarVacio_CuandoTextoEsEspacio()
        {
            string resultado = StringHelper.LimpiarEspacios("   ");

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void LimpiarEspacios_DebeNormalizarEspaciosInternos()
        {
            string resultado = StringHelper.LimpiarEspacios("  Juan    Carlos   Perez  ");

            Assert.Equal("Juan Carlos Perez", resultado);
        }

        [Fact]
        public void QuitarEspacios_DebeRetornarVacio_CuandoTextoEsNull()
        {
            string resultado = StringHelper.QuitarEspacios(null);

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void QuitarEspacios_DebeEliminarTodosLosEspacios()
        {
            string resultado = StringHelper.QuitarEspacios(" correo @ demo .com ");

            Assert.Equal("correo@demo.com", resultado);
        }

        [Fact]
        public void LimpiarTexto_DebeRetornarVacio_CuandoTextoEsEspacio()
        {
            string resultado = StringHelper.LimpiarTexto("   ");

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void LimpiarTexto_DebeNormalizarTexto()
        {
            string resultado = StringHelper.LimpiarTexto("  Ana    Maria   Lopez  ");

            Assert.Equal("Ana Maria Lopez", resultado);
        }

        [Fact]
        public void LimpiarTextoMayus_DebeNormalizarYConvertirAMayusculas()
        {
            string resultado = StringHelper.LimpiarTextoMayus("  lp   central ");

            Assert.Equal("LP CENTRAL", resultado);
        }

        [Fact]
        public void LimpiarTextoMinus_DebeNormalizarYConvertirAMinusculas()
        {
            string resultado = StringHelper.LimpiarTextoMinus("  CORREO   DEMO ");

            Assert.Equal("correo demo", resultado);
        }

        [Fact]
        public void SoloNumeros_DebeRetornarVacio_CuandoTextoEsNull()
        {
            string resultado = StringHelper.SoloNumeros(null);

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void SoloNumeros_DebeConservarSoloDigitos()
        {
            string resultado = StringHelper.SoloNumeros("CI-12 34 AB56");

            Assert.Equal("123456", resultado);
        }

        [Fact]
        public void LimpiarCI_DebeRetornarVacio_CuandoTextoEsEspacio()
        {
            string resultado = StringHelper.LimpiarCI("   ");

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void LimpiarCI_DebeQuitarEspaciosYConvertirAMayusculas()
        {
            string resultado = StringHelper.LimpiarCI(" ab 12 cd ");

            Assert.Equal("AB12CD", resultado);
        }
    }
}
