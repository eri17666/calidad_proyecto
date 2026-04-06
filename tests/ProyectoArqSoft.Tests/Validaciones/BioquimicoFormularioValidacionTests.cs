using ProyectoArqSoft.Models;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests.Validaciones;

public class BioquimicoFormularioValidacionTests
{
    private readonly BioquimicoFormularioValidacion _validador = new();

    [Fact]
    public void Validar_DebeRetornarOk_CuandoLosDatosSonValidos()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "1234567",
            ciExtension: "LP",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarOk_CuandoElCiTieneFormatoConSufijo()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Juan Carlos",
            apellidoMaterno: "Perez",
            apellidoPaterno: "Lopez",
            ci: "1234567-1A",
            ciExtension: "CB",
            telefono: "72345678");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLosNombresNoSonValidos()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Ana1",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "1234567",
            ciExtension: "LP",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El nombre debe tener entre 3 y 45 caracteres y solo letras", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElApellidoMaternoNoEsValido()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Al",
            apellidoPaterno: "Gomez",
            ci: "1234567",
            ciExtension: "LP",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El apellido materno debe tener entre 3 y 45 caracteres y solo letras", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElApellidoPaternoNoEsValido()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: new string('A', 46),
            ci: "1234567",
            ciExtension: "LP",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El apellido paterno debe tener entre 3 y 45 caracteres y solo letras", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElCiEstaVacio()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "",
            ciExtension: "LP",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El número de carnet es obligatorio", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElCiContieneEspacios()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "123 4567",
            ciExtension: "LP",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El número de carnet no debe contener espacios", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElCiNoTieneFormatoValido()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "1234",
            ciExtension: "LP",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El CI debe tener de 5 a 10 dígitos y el formato correcto (ej: 1234567 o 1234567-1A)", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaExtensionDelCiEstaVacia()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "1234567",
            ciExtension: "",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La extensión del CI es obligatoria", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaExtensionDelCiNoEsValida()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "1234567",
            ciExtension: "XX",
            telefono: "71234567");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La extensión del CI no es válida", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElTelefonoEstaVacio()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "1234567",
            ciExtension: "LP",
            telefono: "");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El teléfono es obligatorio", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElTelefonoNoTieneOchoDigitos()
    {
        var bioquimico = CrearBioquimico(
            nombres: "Maria Elena",
            apellidoMaterno: "Flores",
            apellidoPaterno: "Gomez",
            ci: "1234567",
            ciExtension: "LP",
            telefono: "12345ABC");

        Validacion resultado = _validador.Validar(bioquimico);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El teléfono debe tener exactamente 8 dígitos", resultado.Error);
    }

    private static Bioquimico CrearBioquimico(
        string nombres,
        string apellidoMaterno,
        string apellidoPaterno,
        string ci,
        string ciExtension,
        string telefono)
    {
        return new Bioquimico
        {
            Nombres = nombres,
            ApellidoMaterno = apellidoMaterno,
            ApellidoPaterno = apellidoPaterno,
            Ci = ci,
            CiExtencion = ciExtension,
            Telefono = telefono
        };
    }
}
