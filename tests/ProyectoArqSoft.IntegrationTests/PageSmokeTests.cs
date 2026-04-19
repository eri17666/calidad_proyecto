using System.Net;

namespace ProyectoArqSoft.IntegrationTests;

public class PageSmokeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PageSmokeTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Privacy")]
    [InlineData("/Auth/Login")]
    [InlineData("/Error")]
    public async Task Get_PublicPages_ShouldReturnSuccessStatusCode(string url)
    {
        HttpResponseMessage response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_HomePage_ShouldRenderFarmaciaContent()
    {
        HttpResponseMessage response = await _client.GetAsync("/");
        string content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Farmacia VitalCare", content);
        Assert.Contains("Paracetamol", content);
    }
}
