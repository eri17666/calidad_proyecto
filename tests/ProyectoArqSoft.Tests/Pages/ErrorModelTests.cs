using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using ProyectoArqSoft.Pages;
using Xunit;

namespace ProyectoArqSoft.Tests
{
    public class ErrorModelTests
    {
        private readonly Mock<ILogger<ErrorModel>> _mockLogger;

        public ErrorModelTests()
        {
            _mockLogger = new Mock<ILogger<ErrorModel>>();
        }

        [Fact]
        public void ShowRequestId_ReturnsTrue_WhenRequestIdIsSet()
        {
            // Arrange
            var model = new ErrorModel(_mockLogger.Object) { RequestId = "test-id" };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShowRequestId_ReturnsFalse_WhenRequestIdIsEmpty()
        {
            // Arrange
            var model = new ErrorModel(_mockLogger.Object) { RequestId = null };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OnGet_SetsRequestId_FromHttpContext_WhenNoActivity()
        {
            // Arrange
            var model = new ErrorModel(_mockLogger.Object);
            var traceId = "trace-123";

            // Simulamos el HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = traceId;

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            Activity.Current = null; // Aseguramos que no haya actividad

            // Act
            model.OnGet();

            // Assert
            Assert.Equal(traceId, model.RequestId);
        }

        [Fact]
        public void OnGet_SetsRequestId_FromActivity_WhenActivityExists()
        {
            // Arrange
            var model = new ErrorModel(_mockLogger.Object);

            // Creamos e iniciamos una actividad de diagnóstico
            using var activity = new Activity("TestActivity").Start();
            var activityId = activity.Id;

            // Act
            model.OnGet();

            // Assert
            Assert.Equal(activityId, model.RequestId);
        }
    }
}