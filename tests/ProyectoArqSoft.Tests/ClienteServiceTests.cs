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
    public void Actualizar_DebeRetornarOk_CuandoTodoSaleBien()
    {
        var repo = new Mock<IClienteRepository>();
        var val = new Mock<IValidacion<Cliente>>();

        val.Setup(x => x.Validar(It.IsAny<Cliente>()))
           .Returns(Validacion.Ok());

        repo.Setup(x => x.ObtenerPorNit(It.IsAny<string>()))
            .Returns((Cliente?)null);

        repo.Setup(x => x.Update(It.IsAny<Cliente>()))
            .Returns(1);

        var service = new ClienteService(repo.Object, val.Object);

        var result = service.Actualizar(1, false, "123", "Cliente", "correo@test.com");

        Assert.True(result.IsSuccess);
    }
    [Fact]
    public void Actualizar_DebeFallar_CuandoNitDuplicado()
    {
        var repo = new Mock<IClienteRepository>();
        var val = new Mock<IValidacion<Cliente>>();

        val.Setup(x => x.Validar(It.IsAny<Cliente>()))
           .Returns(Validacion.Ok());

        repo.Setup(x => x.ObtenerPorNit("123"))
            .Returns(new Cliente { IdCliente = 2 });

        var service = new ClienteService(repo.Object, val.Object);

        var result = service.Actualizar(1, false, "123", "Cliente", null);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Crear_ConsumidorFinal_DebeAsignarCF()
    {
        var repo = new Mock<IClienteRepository>();
        var val = new Mock<IValidacion<Cliente>>();

        val.Setup(x => x.Validar(It.IsAny<Cliente>()))
           .Returns(Validacion.Ok());

        repo.Setup(x => x.Insert(It.IsAny<Cliente>()))
            .Returns(1);

        var service = new ClienteService(repo.Object, val.Object);

        var result = service.Crear(true, "", "", null);

        Assert.True(result.IsSuccess);

        repo.Verify(x => x.Insert(It.Is<Cliente>(c =>
            c.Nit == "CF" &&
            c.RazonSocial == "Consumidor Final"
        )), Times.Once);
    }
    [Fact]
    public void Eliminar_DebeFallar_CuandoNoElimina()
    {
        var repo = new Mock<IClienteRepository>();
        var val = new Mock<IValidacion<Cliente>>();

        repo.Setup(x => x.Delete(It.IsAny<Cliente>()))
            .Returns(0);

        var service = new ClienteService(repo.Object, val.Object);

        var result = service.Eliminar(1);

        Assert.True(result.IsFailure);
    }
    
}
