using Xunit;
using ProyectoArqSoft.Helpers;

public class StringHelperTests
{
    [Fact]
    public void Limpiar_DebeRetornarTextoSinEspacios()
    {
        var resultado = StringHelper.Limpiar("  hola  ");

        Assert.Equal("hola", resultado);
    }

    [Fact]
    public void Limpiar_DebeRetornarVacio_CuandoNull()
    {
        var resultado = StringHelper.Limpiar(null);

        Assert.Equal("", resultado);
    }

    [Fact]
    public void LimpiarTextoMinus_DebeConvertirAMinusculas()
    {
        var resultado = StringHelper.LimpiarTextoMinus("  HOLA MUNDO  ");

        Assert.Equal("hola mundo", resultado);
    }

    [Fact]
    public void SoloNumeros_DebeEliminarLetras()
    {
        var resultado = StringHelper.SoloNumeros("abc123xyz");

        Assert.Equal("123", resultado);
    }

    [Fact]
    public void SoloNumeros_DebeRetornarVacio_SiNull()
    {
        var resultado = StringHelper.SoloNumeros(null);

        Assert.Equal("", resultado);
    }

    [Fact]
    public void LimpiarCI_DebeQuitarEspaciosYMayus()
    {
        var resultado = StringHelper.LimpiarCI("  123 ab  ");

        Assert.Equal("123AB", resultado);
    }
}
