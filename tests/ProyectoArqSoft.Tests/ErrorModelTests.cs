using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ProyectoArqSoft.Pages;

namespace ProyectoArqSoft.Tests
{
    public class ErrorModelTests
    {
        [Fact]
        public void OnGet_DebeAsignarRequestId()
        {
            var logger = new Mock<ILogger<ErrorModel>>();

            var model = new ErrorModel(logger.Object);

            model.PageContext.HttpContext = new DefaultHttpContext();

            model.OnGet();

            Assert.NotNull(model.RequestId);
        }

        [Fact]
        public void ShowRequestId_DebeRetornarTrue_CuandoExisteRequestId()
        {
            var logger = new Mock<ILogger<ErrorModel>>();

            var model = new ErrorModel(logger.Object)
            {
                RequestId = "123"
            };

            Assert.True(model.ShowRequestId);
        }
    }
}
