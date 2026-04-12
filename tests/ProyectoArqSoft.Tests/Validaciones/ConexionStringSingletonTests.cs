using Xunit;
using ProyectoArqSoft.Services;

namespace ProyectoArqSoft.Tests
{
    public class ConexionStringSingletonTests
    {
        [Fact]
        public void Instancia_DebeRetornarObjetoNoNulo()
        {
            var instancia = ConexionStringSingleton.Instancia;

            Assert.NotNull(instancia);
        }

        [Fact]
        public void Instancia_DebeRetornarSiempreLaMismaReferencia()
        {
            var instancia1 = ConexionStringSingleton.Instancia;
            var instancia2 = ConexionStringSingleton.Instancia;

            Assert.Same(instancia1, instancia2);
        }

        [Fact]
        public void CadenaConexion_NoDebeSerVacia()
        {
            var instancia = ConexionStringSingleton.Instancia;

            Assert.False(string.IsNullOrWhiteSpace(instancia.CadenaConexion));
        }

        [Fact]
        public void CadenaConexion_DebeContenerServerODatabase()
        {
            var instancia = ConexionStringSingleton.Instancia;

            Assert.Contains("server", instancia.CadenaConexion.ToLower());
        }
    }
}
