using BackOffice.Api.Routing;
using BackOffice.Application.DTOs;
using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Api.Controllers {
    [ApiController]
    [Route(ApiRoutes.Auth)]
    public class AuthController : ControllerBase {
        private readonly LoginUseCase loginUseCase;
        private readonly ITokenGenerator tokenGenerator;

        public AuthController(LoginUseCase loginUseCase, ITokenGenerator tokenGenerator) {
            this.loginUseCase = loginUseCase;
            this.tokenGenerator = tokenGenerator;
        }

        [HttpPost(ApiRoutes.Login)]
        public ActionResult Login([FromBody] LoginInput input) {
            try {
                var result = loginUseCase.Execute(input);
                var token = tokenGenerator.Generate(result);

                return Ok(new {
                    result.Id,
                    result.Name,
                    result.Email,
                    Role = result.Role.ToString(),
                    token.AccessToken,
                    token.ExpiresAt
                });
            } catch (ArgumentException exception) {
                return BadRequest(new {
                    Error = exception.Message
                });
            } catch (InvalidOperationException exception) { 
                return Unauthorized(new {
                    Error = exception.Message
                });
            }
        }
    }
}
