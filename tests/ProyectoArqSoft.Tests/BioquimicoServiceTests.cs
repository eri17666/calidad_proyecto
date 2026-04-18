using Moq;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Models;
using ProyectoArqSoft.Services;
using ProyectoArqSoft.Validaciones;
using System.Data;

namespace ProyectoArqSoft.Tests
{
    public class BioquimicoServiceTests
    {
        [Fact]
        public void Crear_DebeFallar_CuandoLaValidacionFalla()
        {
            var repository = new Mock<IBioquimicoRepository>();
            var validador = new Mock<IValidacion<Bioquimico>>();

            validador
                .Setup(x => x.Validar(It.IsAny<Bioquimico>()))
                .Returns(Validacion.Fail("Error de validación"));

            var service = new BioquimicoService(repository.Object, validador.Object);

            var bioquimico = new Bioquimico
            {
                Nombres = "",
                ApellidoPaterno = "",
                ApellidoMaterno = "",
                Ci = "123456",
                CiExtencion = "LP"
            };

            var resultado = service.Crear(bioquimico);

            Assert.True(resultado.IsFailure);
            Assert.Equal("Error de validación", resultado.Error);
            repository.Verify(x => x.Insert(It.IsAny<Bioquimico>()), Times.Never);
        }

        [Fact]
        public void Crear_DebeFallar_CuandoDocumentoEstaDuplicado()
        {
            var repository = new Mock<IBioquimicoRepository>();
            var validador = new Mock<IValidacion<Bioquimico>>();

            validador
                .Setup(x => x.Validar(It.IsAny<Bioquimico>()))
                .Returns(Validacion.Ok());

            repository
                .Setup(x => x.GetByDocumento("123456", "LP"))
                .Returns(new Bioquimico { IdBioquimico = 1 });

            var service = new BioquimicoService(repository.Object, validador.Object);

            var bioquimico = new Bioquimico
            {
                Ci = "123456",
                CiExtencion = "LP"
            };

            var resultado = service.Crear(bioquimico);

            Assert.True(resultado.IsFailure);
            Assert.Contains("Ya existe", resultado.Error);
        }

        [Fact]
        public void Crear_DebeFallar_CuandoInsertNoFunciona()
        {
            var repository = new Mock<IBioquimicoRepository>();
            var validador = new Mock<IValidacion<Bioquimico>>();

            validador
                .Setup(x => x.Validar(It.IsAny<Bioquimico>()))
                .Returns(Validacion.Ok());

            repository
                .Setup(x => x.GetByDocumento(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((Bioquimico?)null);

            repository
                .Setup(x => x.Insert(It.IsAny<Bioquimico>()))
                .Returns(0);

            var service = new BioquimicoService(repository.Object, validador.Object);

            var resultado = service.Crear(new Bioquimico
            {
                Ci = "123456",
                CiExtencion = "LP"
            });

            Assert.True(resultado.IsFailure);
            Assert.Contains("base de datos", resultado.Error);
        }

        [Fact]
        public void Crear_DebeRetornarOk_CuandoTodoSaleBien()
        {
            var repository = new Mock<IBioquimicoRepository>();
            var validador = new Mock<IValidacion<Bioquimico>>();

            validador
                .Setup(x => x.Validar(It.IsAny<Bioquimico>()))
                .Returns(Validacion.Ok());

            repository
                .Setup(x => x.GetByDocumento(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((Bioquimico?)null);

            repository
                .Setup(x => x.Insert(It.IsAny<Bioquimico>()))
                .Returns(1);

            var service = new BioquimicoService(repository.Object, validador.Object);

            var resultado = service.Crear(new Bioquimico
            {
                Ci = "123456",
                CiExtencion = "LP"
            });

            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void Eliminar_DebeRetornarOk_CuandoSeEliminaCorrectamente()
        {
            var repository = new Mock<IBioquimicoRepository>();
            var validador = new Mock<IValidacion<Bioquimico>>();

            repository
                .Setup(x => x.Delete(It.IsAny<Bioquimico>()))
                .Returns(1);

            var service = new BioquimicoService(repository.Object, validador.Object);

            var resultado = service.Eliminar(1);

            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public void ObtenerPorId_DebeRetornarBioquimico()
        {
            var repository = new Mock<IBioquimicoRepository>();
            var validador = new Mock<IValidacion<Bioquimico>>();

            repository
                .Setup(x => x.GetById(1))
                .Returns(new Bioquimico
                {
                    IdBioquimico = 1,
                    Nombres = "Carlos"
                });

            var service = new BioquimicoService(repository.Object, validador.Object);

            var resultado = service.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal("Carlos", resultado.Nombres);
        }

        [Fact]
        public void ObtenerTodos_DebeRetornarTabla()
        {
            var repository = new Mock<IBioquimicoRepository>();
            var validador = new Mock<IValidacion<Bioquimico>>();

            var tabla = new DataTable();

            repository
                .Setup(x => x.GetAll(""))
                .Returns(tabla);

            var service = new BioquimicoService(repository.Object, validador.Object);

            var resultado = service.ObtenerTodos("");

            Assert.NotNull(resultado);
        }

        [Fact]
        public void Actualizar_DebeFallar_CuandoDuplicadoDeOtro()
        {
            var repo = new Mock<IBioquimicoRepository>();
            var val = new Mock<IValidacion<Bioquimico>>();

            val.Setup(x => x.Validar(It.IsAny<Bioquimico>()))
               .Returns(Validacion.Ok());

            repo.Setup(x => x.GetByDocumento(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Bioquimico { IdBioquimico = 2 });

            var service = new BioquimicoService(repo.Object, val.Object);

            var result = service.Actualizar(new Bioquimico
            {
                IdBioquimico = 1,
                Ci = "123",
                CiExtencion = "LP"
            });

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Actualizar_DebePermitir_MismoDocumentoMismoId()
        {
            var repo = new Mock<IBioquimicoRepository>();
            var val = new Mock<IValidacion<Bioquimico>>();

            val.Setup(x => x.Validar(It.IsAny<Bioquimico>()))
               .Returns(Validacion.Ok());

            repo.Setup(x => x.GetByDocumento(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Bioquimico { IdBioquimico = 1 });

            repo.Setup(x => x.Update(It.IsAny<Bioquimico>()))
                .Returns(1);

            var service = new BioquimicoService(repo.Object, val.Object);

            var result = service.Actualizar(new Bioquimico
            {
                IdBioquimico = 1,
                Ci = "123",
                CiExtencion = "LP"
            });

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Eliminar_DebeFallar_CuandoNoSeElimina()
        {
            var repo = new Mock<IBioquimicoRepository>();
            var val = new Mock<IValidacion<Bioquimico>>();

            repo.Setup(x => x.Delete(It.IsAny<Bioquimico>()))
                .Returns(0);

            var service = new BioquimicoService(repo.Object, val.Object);

            var result = service.Eliminar(1);

            Assert.True(result.IsFailure);
        }

    }
}
