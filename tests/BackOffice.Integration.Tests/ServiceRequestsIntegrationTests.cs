using BackOffice.Api;
using BackOffice.Api.Routing;
using BackOffice.Application.Interfaces;
using BackOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace BackOffice.Integration.Tests {
    public class ServiceRequestsIntegrationTests : IClassFixture<WebApplicationFactory<Program>> {
        private const string serviceRequestsRoute = "/" + ApiRoutes.ServiceRequests;
        private readonly WebApplicationFactory<Program> factory;

        public ServiceRequestsIntegrationTests(
            WebApplicationFactory<Program> factory
        ) {
            this.factory = factory;
        }

        private Guid GetDevelopmentUserId() {
            using IServiceScope scope = factory.Services.CreateScope();

            BackOfficeDbContext dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

            IPasswordHasher passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var developmentUser = BackOfficeDatabaseSeeder.SeedDevelopmentUser(dbContext, passwordHasher);

            return developmentUser.Id;
        }

        private async Task<HttpClient> CreateAuthenticatedClientAsync() {
            HttpClient client = factory.CreateClient();

            var input = new {
                email = "diego.dromer@estudoscsharpe.com",
                password = "Senha123!"
            };

            using HttpResponseMessage response = await client.PostAsJsonAsync(
                ApiRoutes.AuthLogin,
                input
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseBody = await response.Content.ReadAsStringAsync();
            using JsonDocument json = JsonDocument.Parse( responseBody );

            string accessToken = json.RootElement
                .GetProperty("accessToken")
                .GetString()!;

            Assert.False(string.IsNullOrWhiteSpace(accessToken));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return client;
        }

        
        [Fact]
        public async Task Post_WhenInputIsValid_ShouldReturnCreated() {

            Guid developmentUserId = GetDevelopmentUserId();

            var input = new {
                title = "Troca de monitor da secretaria",
                createdByUserId = developmentUserId
            };

            HttpClient client = await CreateAuthenticatedClientAsync();

            HttpResponseMessage response = 
                await client.PostAsJsonAsync(
                    serviceRequestsRoute,
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

            HttpClient client = await CreateAuthenticatedClientAsync();

            HttpResponseMessage response = await client.PostAsJsonAsync(
                serviceRequestsRoute,
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

            HttpClient client = await CreateAuthenticatedClientAsync();
            HttpResponseMessage response = await client.PostAsJsonAsync(serviceRequestsRoute, input);

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
            HttpClient client = await CreateAuthenticatedClientAsync();
            Guid createdByUserId = GetDevelopmentUserId();

            string uniquePart = Guid.NewGuid().ToString("N");

            string firstTitle = $"Solicitação GET teste {uniquePart} A";

            string secondTitle = $"Solicitação GET teste {uniquePart} B";

            HttpResponseMessage firstPostResponse = await client.PostAsJsonAsync(
                serviceRequestsRoute,
                new {
                    title = firstTitle,
                    createdByUserId
                }
            );

            HttpResponseMessage secondPostResponse = await client.PostAsJsonAsync(
                serviceRequestsRoute,
                new {
                    title = secondTitle,
                    createdByUserId
                }
            );

            Assert.Equal(HttpStatusCode.Created, firstPostResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Created, secondPostResponse.StatusCode);

            HttpResponseMessage response = await client.GetAsync(serviceRequestsRoute);

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

        [Fact]
        public async Task Get_WhenTokenIsMissing_ShouldReturnUnauthorized() {            
            using HttpClient client = factory.CreateClient();
            using HttpResponseMessage response = await client.GetAsync(serviceRequestsRoute);

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode
            );
        }

        [Fact]
        public async Task Get_WhenTokenSignatureIsTampered_ShouldReturnUnauthorized() {
            using HttpClient client = await CreateAuthenticatedClientAsync();

            string accessToken = client.DefaultRequestHeaders.Authorization!.Parameter!;
            string[] tokenParts = accessToken.Split( '.' );
            char[] signature = tokenParts[2].ToCharArray();

            if (signature[0] == 'A') {
                signature[0] = 'B';
            } else {
                signature[0] = 'A';
            }

            tokenParts[2] = new string(signature);

            string tamperedToken = string.Join(".", tokenParts );

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tamperedToken);

            using HttpResponseMessage response = await client.GetAsync(serviceRequestsRoute);

            Assert.Equal(
                HttpStatusCode.Unauthorized, 
                response.StatusCode
            );
        }
    }
}
