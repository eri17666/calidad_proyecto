using Moq;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Models;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;
using System.Data;

namespace ProyectoArqSoft.Tests;

public class MedicamentoServiceTests
{
    [Fact]
    public void Crear_DebeFallar_CuandoLaValidacionFalla()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Medicamento>()))
            .Returns(Validacion.Fail("Error de validacion."));

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            nombre: "Paracetamol",
            presentacion: "Tabletas",
            clasificacion: "Analgesico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Assert.True(resultado.IsFailure);
        Assert.Equal("Error de validacion.", resultado.Error);
        repository.Verify(x => x.Insert(It.IsAny<Medicamento>()), Times.Never);
    }

    [Fact]
    public void Crear_DebeFallar_CuandoElRepositorioNoInserta()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Medicamento>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.Insert(It.IsAny<Medicamento>()))
            .Returns(0);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            nombre: "Paracetamol",
            presentacion: "Tabletas",
            clasificacion: "Analgesico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Assert.True(resultado.IsFailure);
        Assert.Equal("No se pudo registrar el medicamento.", resultado.Error);
        repository.Verify(x => x.Insert(It.IsAny<Medicamento>()), Times.Once);
    }

    [Fact]
    public void Crear_DebeRetornarOk_CuandoTodoSaleBien()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Medicamento>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.Insert(It.IsAny<Medicamento>()))
            .Returns(1);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            nombre: "Paracetamol",
            presentacion: "Tabletas",
            clasificacion: "Analgesico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
        repository.Verify(x => x.Insert(It.IsAny<Medicamento>()), Times.Once);
    }

    [Fact]
    public void Crear_DebeLimpiarLosCampos_AntesDeValidarEInsertar()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        Medicamento? medicamentoValidado = null;
        Medicamento? medicamentoInsertado = null;

        validador
            .Setup(x => x.Validar(It.IsAny<Medicamento>()))
            .Callback<Medicamento>(m => medicamentoValidado = m)
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.Insert(It.IsAny<Medicamento>()))
            .Callback<Medicamento>(m => medicamentoInsertado = m)
            .Returns(1);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.Crear(
            nombre: "  Paracetamol   500  ",
            presentacion: "  Tabletas  ",
            clasificacion: "  Analgesico  ",
            concentracion: "  500 mg  ",
            precio: 12.5m,
            stock: 50);

        Assert.True(resultado.IsSuccess);

        Assert.NotNull(medicamentoValidado);
        Assert.Equal("Paracetamol 500", medicamentoValidado!.Nombre);
        Assert.Equal("Tabletas", medicamentoValidado.Presentacion);
        Assert.Equal("Analgesico", medicamentoValidado.Clasificacion);
        Assert.Equal("500 mg", medicamentoValidado.Concentracion);

        Assert.NotNull(medicamentoInsertado);
        Assert.Equal("Paracetamol 500", medicamentoInsertado!.Nombre);
        Assert.Equal("Tabletas", medicamentoInsertado.Presentacion);
        Assert.Equal("Analgesico", medicamentoInsertado.Clasificacion);
        Assert.Equal("500 mg", medicamentoInsertado.Concentracion);
    }

    [Fact]
    public void Actualizar_DebeFallar_CuandoLaValidacionFalla()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Medicamento>()))
            .Returns(Validacion.Fail("Error de validacion."));

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.Actualizar(
            id: 7,
            nombre: "Paracetamol",
            presentacion: "Tabletas",
            clasificacion: "Analgesico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Assert.True(resultado.IsFailure);
        Assert.Equal("Error de validacion.", resultado.Error);
        repository.Verify(x => x.Update(It.IsAny<Medicamento>()), Times.Never);
    }

    [Fact]
    public void Actualizar_DebeFallar_CuandoElRepositorioNoActualiza()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Medicamento>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.Update(It.IsAny<Medicamento>()))
            .Returns(0);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.Actualizar(
            id: 7,
            nombre: "Paracetamol",
            presentacion: "Tabletas",
            clasificacion: "Analgesico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Assert.True(resultado.IsFailure);
        Assert.Equal("No se pudo actualizar el medicamento.", resultado.Error);
        repository.Verify(x => x.Update(It.IsAny<Medicamento>()), Times.Once);
    }

    [Fact]
    public void Actualizar_DebeRetornarOk_CuandoTodoSaleBien()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        validador
            .Setup(x => x.Validar(It.IsAny<Medicamento>()))
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.Update(It.IsAny<Medicamento>()))
            .Returns(1);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.Actualizar(
            id: 7,
            nombre: "Paracetamol",
            presentacion: "Tabletas",
            clasificacion: "Analgesico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
        repository.Verify(x => x.Update(It.IsAny<Medicamento>()), Times.Once);
    }

    [Fact]
    public void Actualizar_DebeLimpiarLosCampos_AntesDeValidarYActualizar()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        Medicamento? medicamentoValidado = null;
        Medicamento? medicamentoActualizado = null;

        validador
            .Setup(x => x.Validar(It.IsAny<Medicamento>()))
            .Callback<Medicamento>(m => medicamentoValidado = m)
            .Returns(Validacion.Ok());

        repository
            .Setup(x => x.Update(It.IsAny<Medicamento>()))
            .Callback<Medicamento>(m => medicamentoActualizado = m)
            .Returns(1);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.Actualizar(
            id: 7,
            nombre: "  Paracetamol   500  ",
            presentacion: "  Tabletas  ",
            clasificacion: "  Analgesico  ",
            concentracion: "  500 mg  ",
            precio: 12.5m,
            stock: 50);

        Assert.True(resultado.IsSuccess);

        Assert.NotNull(medicamentoValidado);
        Assert.Equal(7, medicamentoValidado!.Id);
        Assert.Equal("Paracetamol 500", medicamentoValidado.Nombre);
        Assert.Equal("Tabletas", medicamentoValidado.Presentacion);
        Assert.Equal("Analgesico", medicamentoValidado.Clasificacion);
        Assert.Equal("500 mg", medicamentoValidado.Concentracion);

        Assert.NotNull(medicamentoActualizado);
        Assert.Equal(7, medicamentoActualizado!.Id);
        Assert.Equal("Paracetamol 500", medicamentoActualizado.Nombre);
        Assert.Equal("Tabletas", medicamentoActualizado.Presentacion);
        Assert.Equal("Analgesico", medicamentoActualizado.Clasificacion);
        Assert.Equal("500 mg", medicamentoActualizado.Concentracion);
    }

    [Fact]
    public void EliminarLogicamente_DebeFallar_CuandoElRepositorioNoElimina()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        repository
            .Setup(x => x.Delete(It.IsAny<Medicamento>()))
            .Returns(0);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.EliminarLogicamente(7);

        Assert.True(resultado.IsFailure);
        Assert.Equal("No se pudo eliminar el medicamento.", resultado.Error);
        repository.Verify(x => x.Delete(It.IsAny<Medicamento>()), Times.Once);
    }

    [Fact]
    public void EliminarLogicamente_DebeRetornarOk_CuandoElRepositorioElimina()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        repository
            .Setup(x => x.Delete(It.IsAny<Medicamento>()))
            .Returns(1);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.EliminarLogicamente(7);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
        repository.Verify(x => x.Delete(It.IsAny<Medicamento>()), Times.Once);
    }

    [Fact]
    public void EliminarLogicamente_DebeEnviarElIdCorrecto_AlRepositorio()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        Medicamento? medicamentoEliminado = null;

        repository
            .Setup(x => x.Delete(It.IsAny<Medicamento>()))
            .Callback<Medicamento>(m => medicamentoEliminado = m)
            .Returns(1);

        var service = new MedicamentoService(repository.Object, validador.Object);

        Validacion resultado = service.EliminarLogicamente(7);

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(medicamentoEliminado);
        Assert.Equal(7, medicamentoEliminado!.Id);
    }

    [Fact]
    public void ObtenerTodos_DebeRetornarDataTable()
    {
        var repo = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        var tabla = new DataTable();

        repo.Setup(x => x.GetAll()).Returns(tabla);

        var service = new MedicamentoService(repo.Object, validador.Object);

        var resultado = service.ObtenerTodos();

        Assert.NotNull(resultado);
        Assert.Equal(tabla, resultado);
    }

    [Fact]
    public void ObtenerTodosConFiltro_DebeRetornarDataTable()
    {
        var repo = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        var tabla = new DataTable();

        repo.Setup(x => x.GetAll("para")).Returns(tabla);

        var service = new MedicamentoService(repo.Object, validador.Object);

        var resultado = service.ObtenerTodos("para");

        Assert.NotNull(resultado);
        Assert.Equal(tabla, resultado);
    }

    [Fact]
    public void ObtenerPorId_DebeRetornarMedicamento()
    {
        var repo = new Mock<IMedicamentoRepository>();
        var validador = new Mock<IValidacion<Medicamento>>();

        var medicamento = new Medicamento
        {
            Id = 1,
            Nombre = "Paracetamol"
        };

        repo.Setup(x => x.GetById(1)).Returns(medicamento);

        var service = new MedicamentoService(repo.Object, validador.Object);

        var resultado = service.ObtenerPorId(1);

        Assert.NotNull(resultado);
        Assert.Equal("Paracetamol", resultado.Nombre);
    }
}
