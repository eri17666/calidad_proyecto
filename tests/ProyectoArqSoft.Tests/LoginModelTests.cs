using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace ProyectoArqSoft.Tests
{
    public class LoginModelTests
    {
        private LoginModel CrearModeloConSession(Mock<ISession> sessionMock)
        {
            var model = new LoginModel();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            return model;
        }

        [Fact]
        public void OnPost_DebeRetornarPage_CuandoModelStateEsInvalido()
        {
            var model = new LoginModel();

            model.ModelState.AddModelError("Usuario", "Requerido");

            var result = model.OnPost();

            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void OnPost_DebeRedireccionar_CuandoCredencialesSonCorrectas()
        {
            var sessionMock = new Mock<ISession>();

            var model = CrearModeloConSession(sessionMock);
            model.Usuario = "admin";
            model.Password = "1234";

            var result = model.OnPost();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);

            sessionMock.Verify(s => s.Set(
                "Usuario",
                It.IsAny<byte[]>()),
                Times.Once);
        }

        [Fact]
        public void OnPost_DebeRetornarPage_CuandoCredencialesIncorrectas()
        {
            var sessionMock = new Mock<ISession>();

            var model = CrearModeloConSession(sessionMock);
            model.Usuario = "admin";
            model.Password = "wrong";

            var result = model.OnPost();

            Assert.IsType<PageResult>(result);
            Assert.False(model.ModelState.IsValid);
        }
    }
}
