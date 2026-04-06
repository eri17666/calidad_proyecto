using ProyectoArqSoft.Models;
using ProyectoArqSoft.Validaciones;

namespace ProyectoArqSoft.Tests.Validaciones;

public class MedicamentoValidacionTests
{
    private readonly MedicamentoValidacion _validador = new();

    [Fact]
    public void Validar_DebeRetornarOk_CuandoLosDatosSonValidos()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsSuccess);
        Assert.Equal(string.Empty, resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNombreEstaVacio()
    {
        var medicamento = CrearMedicamento(
            nombre: "",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El nombre es obligatorio.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNombreContieneCaracteresInvalidos()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol@500",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El nombre contiene caracteres inválidos o no tiene un formato correcto.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNombreTieneMenosDeTresCaracteres()
    {
        var medicamento = CrearMedicamento(
            nombre: "AB",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El nombre contiene caracteres inválidos o no tiene un formato correcto.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNombreTieneMasDeCienCaracteres()
    {
        var medicamento = CrearMedicamento(
            nombre: new string('A', 101),
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El nombre contiene caracteres inválidos o no tiene un formato correcto.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElNombreEstaFormadoPorUnSoloCaracterRepetido()
    {
        var medicamento = CrearMedicamento(
            nombre: "AAAA",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El nombre contiene caracteres inválidos o no tiene un formato correcto.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaPresentacionEstaVacia()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La presentación es obligatoria.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaClasificacionEstaVacia()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "Tabletas",
            clasificacion: "",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La clasificación es obligatoria.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaConcentracionEstaVacia()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La concentración es obligatoria.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoLaConcentracionTieneFormatoInvalido()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "abc",
            precio: 12.5m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("La concentración no tiene un formato válido. Ejemplos: 500 mg, 250 mg/5ml, 0.9 %.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElPrecioEsMenorOIgualACero()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 0m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El precio debe ser mayor a 0 Bs.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElPrecioEsMayorAMil()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 1000.01m,
            stock: 50);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El precio no puede ser mayor a 1000 Bs.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElStockEsNegativo()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: -1);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El stock no puede ser negativo.", resultado.Error);
    }

    [Fact]
    public void Validar_DebeRetornarError_CuandoElStockEsMayorACienMil()
    {
        var medicamento = CrearMedicamento(
            nombre: "Paracetamol 500",
            presentacion: "Tabletas",
            clasificacion: "Analgésico",
            concentracion: "500 mg",
            precio: 12.5m,
            stock: 100001);

        Validacion resultado = _validador.Validar(medicamento);

        Assert.True(resultado.IsFailure);
        Assert.Equal("El stock no puede ser mayor a 100000 items.", resultado.Error);
    }

    private static Medicamento CrearMedicamento(
        string nombre,
        string presentacion,
        string clasificacion,
        string concentracion,
        decimal precio,
        int stock)
    {
        return new Medicamento
        {
            Nombre = nombre,
            Presentacion = presentacion,
            Clasificacion = clasificacion,
            Concentracion = concentracion,
            Precio = precio,
            Stock = stock
        };
    }
}
