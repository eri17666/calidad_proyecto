using Moq;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Models;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests;

public class ClienteServiceTests
{
    [Fact]
    public void Crear_DebeFallar_CuandoLaValidacionFalla()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Returns(Validacion.Fail("Error de validacion."));

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            esConsumidorFinal: false,
            nit: "123456",
            razonSocial: "Cliente Prueba",
            correoElectronico: "cliente@correo.com");

        Assert.True(resultado.IsFailure);
        Assert.Equal("Error de validacion.", resultado.Error);
        repository.Verify(x => x.ObtenerPorNit(It.IsAny<string>()), Times.Never);
        repository.Verify(x => x.Insert(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public void Crear_DebeFallar_CuandoYaExisteUnClienteConElMismoNit()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.ObtenerPorNit("123456"))
            .Returns(new Cliente
            {
                IdCliente = 8,
                Nit = "123456",
                RazonSocial = "Cliente Existente"
            });

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            esConsumidorFinal: false,
            nit: "123456",
            razonSocial: "Nuevo Cliente",
            correoElectronico: "nuevo@correo.com");

        Assert.True(resultado.IsFailure);
        Assert.Equal("Ya existe un cliente con ese NIT.", resultado.Error);
        repository.Verify(x => x.Insert(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public void Crear_DebeFallar_CuandoElRepositorioNoInserta()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.ObtenerPorNit("123456"))
            .Returns((Cliente?)null);

        repository
            .Setup(x => x.Insert(It.IsAny<Cliente>()))
            .Returns(0);

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            esConsumidorFinal: false,
            nit: "123456",
            razonSocial: "Cliente Nuevo",
            correoElectronico: "nuevo@correo.com");

        Assert.True(resultado.IsFailure);
        Assert.Equal("No se pudo registrar el cliente.", resultado.Error);
        repository.Verify(x => x.Insert(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public void Crear_DebeRetornarOk_CuandoTodoSaleBien()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.ObtenerPorNit("123456"))
            .Returns((Cliente?)null);

        repository
            .Setup(x => x.Insert(It.IsAny<Cliente>()))
            .Returns(1);

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            esConsumidorFinal: false,
            nit: "123456",
            razonSocial: "Cliente Nuevo",
            correoElectronico: "nuevo@correo.com");

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
        repository.Verify(x => x.Insert(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public void Actualizar_DebeFallar_CuandoLaValidacionFalla()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Returns(Validacion.Fail("Error de validacion."));

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Actualizar(
            id: 3,
            esConsumidorFinal: false,
            nit: "123456",
            razonSocial: "Cliente Prueba",
            correoElectronico: "cliente@correo.com");

        Assert.True(resultado.IsFailure);
        Assert.Equal("Error de validacion.", resultado.Error);
        repository.Verify(x => x.ObtenerPorNit(It.IsAny<string>()), Times.Never);
        repository.Verify(x => x.Update(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public void Actualizar_DebeFallar_CuandoElRepositorioNoActualiza()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.ObtenerPorNit("123456"))
            .Returns((Cliente?)null);

        repository
            .Setup(x => x.Update(It.IsAny<Cliente>()))
            .Returns(0);

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Actualizar(
            id: 3,
            esConsumidorFinal: false,
            nit: "123456",
            razonSocial: "Cliente Actualizado",
            correoElectronico: "actualizado@correo.com");

        Assert.True(resultado.IsFailure);
        Assert.Equal("No se pudo actualizar el cliente.", resultado.Error);
        repository.Verify(x => x.Update(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public void Actualizar_DebeRetornarOk_CuandoElNitPerteneceAlMismoCliente()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.ObtenerPorNit("123456"))
            .Returns(new Cliente
            {
                IdCliente = 7,
                Nit = "123456",
                RazonSocial = "Cliente Existente"
            });

        Cliente? clienteActualizado = null;
        repository
            .Setup(x => x.Update(It.IsAny<Cliente>()))
            .Callback<Cliente>(cliente => clienteActualizado = cliente)
            .Returns(1);

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Actualizar(
            id: 7,
            esConsumidorFinal: false,
            nit: "123456",
            razonSocial: "Cliente Actualizado",
            correoElectronico: "actualizado@correo.com");

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(clienteActualizado);
        Assert.Equal(7, clienteActualizado!.IdCliente);
        repository.Verify(x => x.Update(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public void Eliminar_DebeFallar_CuandoElRepositorioNoElimina()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        repository
            .Setup(x => x.Delete(It.IsAny<Cliente>()))
            .Returns(0);

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Eliminar(9);

        Assert.True(resultado.IsFailure);
        Assert.Equal("No se pudo eliminar el cliente.", resultado.Error);
    }

    [Fact]
    public void Eliminar_DebeRetornarOk_CuandoElRepositorioEliminaYDebeEnviarElIdCorrecto()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        repository
            .Setup(x => x.Delete(It.IsAny<Cliente>()))
            .Returns(1);

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Eliminar(11);

        Assert.True(resultado.IsSuccess);
        repository.Verify(
            x => x.Delete(It.Is<Cliente>(cliente => cliente.IdCliente == 11)),
            Times.Once);
    }

    [Fact]
    public void Crear_DebeUsarConsumidorFinal_YNoConsultarDuplicado_CuandoEsConsumidorFinal()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        Cliente? clienteValidado = null;
        Cliente? clienteInsertado = null;

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Callback<Cliente>(cliente => clienteValidado = cliente)
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.Insert(It.IsAny<Cliente>()))
            .Callback<Cliente>(cliente => clienteInsertado = cliente)
            .Returns(1);

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            esConsumidorFinal: true,
            nit: " 999999 ",
            razonSocial: " Cliente Manual ",
            correoElectronico: " correo@demo.com ");

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(clienteValidado);
        Assert.Equal("CF", clienteValidado!.Nit);
        Assert.Equal("Consumidor Final", clienteValidado.RazonSocial);
        Assert.NotNull(clienteInsertado);
        Assert.Equal("CF", clienteInsertado!.Nit);
        Assert.Equal("Consumidor Final", clienteInsertado.RazonSocial);
        repository.Verify(x => x.ObtenerPorNit(It.IsAny<string>()), Times.Never);
        repository.Verify(x => x.Insert(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public void Crear_DebeLimpiarCamposAntesDeValidarYGuardar()
    {
        var repository = new Mock<IClienteRepository>();
        var validador = new Mock<IValidacion<Cliente>>();

        Cliente? clienteValidado = null;
        Cliente? clienteInsertado = null;

        validador
            .Setup(x => x.Validar(It.IsAny<Cliente>()))
            .Callback<Cliente>(cliente => clienteValidado = cliente)
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.ObtenerPorNit("123456"))
            .Returns((Cliente?)null);

        repository
            .Setup(x => x.Insert(It.IsAny<Cliente>()))
            .Callback<Cliente>(cliente => clienteInsertado = cliente)
            .Returns(1);

        var service = new ClienteService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            esConsumidorFinal: false,
            nit: " 12 34 56 ",
            razonSocial: "  Cliente    Nuevo  ",
            correoElectronico: " correo @demo.com ");

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(clienteValidado);
        Assert.Equal("123456", clienteValidado!.Nit);
        Assert.Equal("Cliente Nuevo", clienteValidado.RazonSocial);
        Assert.Equal("correo@demo.com", clienteValidado.CorreoElectronico);
        Assert.NotNull(clienteInsertado);
        Assert.Equal("123456", clienteInsertado!.Nit);
        Assert.Equal("Cliente Nuevo", clienteInsertado.RazonSocial);
        Assert.Equal("correo@demo.com", clienteInsertado.CorreoElectronico);
        repository.Verify(x => x.ObtenerPorNit("123456"), Times.Once);
    }
}
