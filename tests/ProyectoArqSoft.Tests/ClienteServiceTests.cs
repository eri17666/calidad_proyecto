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
}
