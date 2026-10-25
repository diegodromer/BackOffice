using BackOffice.Api;
using BackOffice.Api.Routing;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BackOffice.Integration.Tests {
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>> {
        private const string loginRoute = ApiRoutes.AuthLogin;
        private readonly WebApplicationFactory<Program> factory;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory) {
            this.factory = factory;
        }

        [Fact]
        public async Task Post_WhenCredentialsAreValid_ShouldReturnOkAndUserData() {
            HttpClient client = factory.CreateClient();

            var input = new {
                email = "diego.dromer@estudoscsharpe.com",
                password = "Senha123!"
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(
                loginRoute,
                input
            );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseBody = await response.Content.ReadAsStringAsync();
            using JsonDocument json =  JsonDocument.Parse( responseBody );

            Assert.NotEqual(
                Guid.Empty,
                json.RootElement.GetProperty("id").GetGuid()
            );

            Assert.Equal(
                "Diego",
                json.RootElement.GetProperty("name").GetString()
            );

            Assert.Equal(
                input.email,
                json.RootElement.GetProperty("email").GetString()
            );

            Assert.Equal(
                "Admin",
                json.RootElement.GetProperty("role").GetString()
            );

            Assert.False(
                json.RootElement.TryGetProperty("passwordHash", out _)
            );
        }

        [Fact]
        public async Task Post_WhenPasswordIsIncorrect_ShouldReturnUnauthorized() {
            HttpClient client = factory.CreateClient();

            var input = new {
                email = "diego.dromer@estudoscsharpe.com",
                password = "SenhaErrada123!"
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(
                loginRoute,
                input
            );

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode
            );

            string responseBody = await response.Content.ReadAsStringAsync();
            using JsonDocument json = JsonDocument.Parse(responseBody);

            Assert.Equal(
                "E-mail ou senha inválidos.",
                json.RootElement.GetProperty("error").GetString()
            );
        }

        [Fact]
        public async Task Post_WhenPasswordIsEmpty_ShouldReturnBadRequest() {
            HttpClient client = factory.CreateClient(); 

            var input = new {
                email = "diego.dromer@estudoscsharpe.com",
                password = string.Empty
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(
                loginRoute,
                input
            );

            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode
            );

            string responseBody = await response.Content.ReadAsStringAsync();
            using JsonDocument json = JsonDocument.Parse(responseBody);

            Assert.StartsWith(
                "A senha é obrigatória.",
                json.RootElement.GetProperty("error").GetString()!
            );
        }
    }
}

