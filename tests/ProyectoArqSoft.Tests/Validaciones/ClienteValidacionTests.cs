using ProyectoArqSoft.Models;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests.Validaciones;

public class ClienteValidacionTests
{
    private readonly ClienteValidacion _validador = new();

    [Fact]
    public void Validar_DebeRetornarOk_CuandoElClienteEsConsumidorFinal()
    {
        var cliente = CrearCliente(nit: "CF", razonSocial: "Consumidor Final", correoElectronico: "");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarOk_CuandoLosDatosSonValidos()
    {
        var cliente = CrearCliente(nit: "12345", razonSocial: "Cliente Valido", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarOk_CuandoElCorreoElectronicoEstaVacio()
    {
        var cliente = CrearCliente(nit: "12345", razonSocial: "Cliente Valido", correoElectronico: "");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNitEstaVacio()
    {
        var cliente = CrearCliente(nit: "", razonSocial: "Cliente Valido", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El NIT es obligatorio.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNitContieneEspacios()
    {
        var cliente = CrearCliente(nit: "12 345", razonSocial: "Cliente Valido", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El NIT no debe contener espacios.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNitContieneCaracteresNoNumericos()
    {
        var cliente = CrearCliente(nit: "12A45", razonSocial: "Cliente Valido", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El NIT solo debe contener numeros y el formato correcto (ej: 123456789).", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNitTieneMenosDeCincoDigitos()
    {
        var cliente = CrearCliente(nit: "1234", razonSocial: "Cliente Valido", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El NIT debe tener de 5 a 20 digitos y el formato correcto (ej: 123456789).", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNitTieneMasDeVeinteDigitos()
    {
        var cliente = CrearCliente(nit: "123456789012345678901", razonSocial: "Cliente Valido", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El NIT debe tener de 5 a 20 digitos y el formato correcto (ej: 123456789).", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNitEstaCompuestoSoloPorCeros()
    {
        var cliente = CrearCliente(nit: "00000", razonSocial: "Cliente Valido", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El NIT no puede estar compuesto solo por ceros.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaRazonSocialEstaVacia()
    {
        var cliente = CrearCliente(nit: "12345", razonSocial: "", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La razon social es obligatoria.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaRazonSocialTieneMenosDeTresCaracteres()
    {
        var cliente = CrearCliente(nit: "12345", razonSocial: "AB", correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La razon social debe tener entre 3 y 45 caracteres.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaRazonSocialTieneMasDeCuarentaYCincoCaracteres()
    {
        var cliente = CrearCliente(nit: "12345", razonSocial: new string('A', 46), correoElectronico: "cliente@correo.com");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La razon social debe tener entre 3 y 45 caracteres.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElCorreoElectronicoSuperaLaLongitudPermitida()
    {
        var correoLargo = $"{new string('a', 40)}@ab.com";
        var cliente = CrearCliente(nit: "12345", razonSocial: "Cliente Valido", correoElectronico: correoLargo);

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El correo electronico no puede superar los 45 caracteres.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElCorreoElectronicoTieneFormatoInvalido()
    {
        var cliente = CrearCliente(nit: "12345", razonSocial: "Cliente Valido", correoElectronico: "correo-invalido");

        Validacion resultado = _validador.Validar(cliente);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El correo electronico no tiene un formato valido.", resultado.Error);
    }

    private static Cliente CrearCliente(string nit, string razonSocial, string correoElectronico)
    {
        return new Cliente
        {
            Nit = nit,
            RazonSocial = razonSocial,
            CorreoElectronico = correoElectronico
        };
    }
}
