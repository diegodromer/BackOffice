using BackOffice.Api;
using BackOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace BackOffice.Integration.Tests {
    public class ServiceRequestsIntegrationTests : IClassFixture<WebApplicationFactory<Program>> {
        private readonly WebApplicationFactory<Program> factory;

        public ServiceRequestsIntegrationTests(
            WebApplicationFactory<Program> factory
        ) {
            this.factory = factory;
        }

        private Guid GetDevelopmentUserId() {
            using IServiceScope scope = factory.Services.CreateScope();

            BackOfficeDbContext dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

            var developmentUser = BackOfficeDatabaseSeeder.SeedDevelopmentUser(dbContext);

            return developmentUser.Id;
        }
        
        [Fact]
        public async Task Post_WhenInputIsValid_ShouldReturnCreated() {

            Guid developmentUserId = GetDevelopmentUserId();

            var input = new {
                title = "Troca de monitor da secretaria",
                createdByUserId = developmentUserId
            };

            HttpClient client = factory.CreateClient();

            HttpResponseMessage response = 
                await client.PostAsJsonAsync(
                    "/api/ServiceRequests",
                    input
                );

            string responseBody = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"HTTP {(int)response.StatusCode} ({response.StatusCode})\nCorpo: {responseBody}"
            );

            using JsonDocument json = JsonDocument.Parse(responseBody);

            Assert.Equal("Troca de monitor da secretaria", json.RootElement.GetProperty("title").GetString());

            Assert.Equal(
                "Open",
                json.RootElement.GetProperty("status").GetString()
            );

            Assert.True(
                json.RootElement.TryGetProperty(
                    "createdById",
                    out JsonElement createdById
                ),
                $"O JSON não trouxe a propriedade 'createdById'. Corpo: {responseBody}" 
            );

            Assert.Equal(
                developmentUserId.ToString(),
                createdById.ToString()
            );
        }

        [Fact]
        public async Task Post_WhenCreatedByUserDoesNotExist_ShouldReturnBadRequest() {
            var input = new {
                title = "Solicitação com usuário inexistente",
                createdByUserId = Guid.NewGuid(),
            };

            HttpClient client = factory.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "/api/ServiceRequests",
                input
            );

            string responseBody = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                $"HTTP {(int)response.StatusCode} ({response.StatusCode})\nCorpo: {responseBody}"
            );

            using JsonDocument json = JsonDocument.Parse(responseBody);

            Assert.Equal(
                "O usuário que abriu a solicitação não foi encontrado.",
                json.RootElement.GetProperty("error").GetString()
            );
        }

        [Fact]
        public async Task Post_WhenTitleIsBlank_ShouldReturnBadRequest() {
            Guid developmentUserId = GetDevelopmentUserId();

            var input = new {
                title = " ",
                createdByUserId = developmentUserId
            };

            HttpClient client = factory.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/ServiceRequests", input);

            string responseBody = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                $"HTTP {(int)response.StatusCode} ({response.StatusCode})\nCorpo: {responseBody}"
            );

            using JsonDocument json = JsonDocument.Parse(responseBody);

            string error = json.RootElement.GetProperty("error").GetString()!;

            Assert.StartsWith(
                "O título da solicitação é obrigatório.",
                error
            );
        }

        [Fact]
        public async Task Get_WhenRequestsExist_ShouldReturnOkAndCreatedRequests() { 
            HttpClient client = factory.CreateClient();

            Guid createdByUserId = GetDevelopmentUserId();

            string uniquePart = Guid.NewGuid().ToString("N");

            string firstTitle = $"Solicitação GET teste {uniquePart} A";

            string secondTitle = $"Solicitação GET teste {uniquePart} B";

            HttpResponseMessage firstPostResponse = await client.PostAsJsonAsync(
                "/api/ServiceRequests",
                new {
                    title = firstTitle,
                    createdByUserId
                }
            );

            HttpResponseMessage secondPostResponse = await client.PostAsJsonAsync(
                "/api/ServiceRequests",
                new {
                    title = secondTitle,
                    createdByUserId
                }
            );

            Assert.Equal(HttpStatusCode.Created, firstPostResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Created, secondPostResponse.StatusCode);

            HttpResponseMessage response = await client.GetAsync("/api/ServiceRequests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseBody = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(responseBody);

            JsonElement serviceRequests = document.RootElement;
            
            Assert.Equal(JsonValueKind.Array, serviceRequests.ValueKind);

            Assert.Contains(
                serviceRequests.EnumerateArray(),
                serviceRequests => serviceRequests.GetProperty("title").GetString() == firstTitle
            );
            Assert.Contains(
                serviceRequests.EnumerateArray(),
                serviceRequests => serviceRequests.GetProperty("title").GetString() == secondTitle
            );
        }    
    }
}
