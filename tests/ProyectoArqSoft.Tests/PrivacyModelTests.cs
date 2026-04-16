using Microsoft.Extensions.Logging;
using Moq;
using ProyectoArqSoft.Pages;

namespace ProyectoArqSoft.Tests
{
    public class PrivacyModelTests
    {
        [Fact]
        public void OnGet_DebeEjecutarseSinError()
        {
            var logger = new Mock<ILogger<PrivacyModel>>();

            var model = new PrivacyModel(logger.Object);

            model.OnGet();

            Assert.NotNull(model);
        }
    }
}
