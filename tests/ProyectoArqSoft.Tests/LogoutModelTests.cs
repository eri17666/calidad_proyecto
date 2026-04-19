using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ProyectoArqSoft.Pages.Auth;
using Xunit;

namespace ProyectoArqSoft.Tests
{
    public class LogoutModelTests
    {
        [Fact]
        public void OnGet_DebeLimpiarSession_YRedireccionarAIndex()
        {
            // Arrange
            var sessionMock = new Mock<ISession>();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;

            var model = new LogoutModel
            {
                PageContext = new PageContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = model.OnGet();

            // Assert
            sessionMock.Verify(s => s.Clear(), Times.Once);

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
        }
    }
}
