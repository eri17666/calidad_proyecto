using Moq;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Models;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests;

public class ClienteServiceTests
{
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
}
