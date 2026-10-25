using BackOffice.Api.Routing;
using BackOffice.Application.DTOs;
using BackOffice.Application.UseCases;
using BackOffice.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackOffice.Api.Controllers {
    [Authorize]
    [ApiController]
    [Route(ApiRoutes.ServiceRequests)]
    public class ServiceRequestsController : ControllerBase {
        private readonly CreateServiceRequestUseCase createServiceRequestUseCase;
        private readonly ListServiceRequestsUseCase listServiceRequestsUseCase;
        private readonly GetServiceRequestByIdUseCase getServiceRequestByIdUseCase;

        public ServiceRequestsController(
            CreateServiceRequestUseCase createServiceRequestUseCase,
            ListServiceRequestsUseCase listServiceRequestsUseCase,
            GetServiceRequestByIdUseCase getServiceRequestByIdUseCase

        ) {
            this.createServiceRequestUseCase = createServiceRequestUseCase;
            this.listServiceRequestsUseCase = listServiceRequestsUseCase;
            this.getServiceRequestByIdUseCase = getServiceRequestByIdUseCase;
        }

        [HttpPost]
        public ActionResult Create(
                [FromBody] CreateServiceRequestInput input
            ) {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty) {
                return Unauthorized();
            }

            string? userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (!Enum.TryParse(userRoleClaim, out UserRole userRole) || !Enum.IsDefined(userRole)) {
                return Forbid();
            }

            try {
                var serviceRequest = createServiceRequestUseCase.Execute(input, userId, userRole);

                return Created(
                    $"/{ApiRoutes.ServiceRequests}/{serviceRequest.Id}",
                    new {
                        serviceRequest.Id,
                        serviceRequest.Title,
                        Status = serviceRequest.Status.ToString(),
                        CreatedById = serviceRequest.CreatedBy.Id
                    }
                );
            } catch (ArgumentException exception) {
                return BadRequest(new {
                    Error = exception.Message
                });
            } catch (InvalidOperationException exception) {
                return BadRequest(new {
                    Error = exception.Message
                });
            } catch (UnauthorizedAccessException) {
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult Get() {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty) {
                return Unauthorized();
            }

            string? userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (!Enum.TryParse(userRoleClaim, out UserRole userRole) || !Enum.IsDefined(userRole)) {
                return Forbid();
            }

            var serviceRequests = listServiceRequestsUseCase.Execute(userId, userRole);

            return Ok(
                serviceRequests.Select(
                    serviceRequest => new {
                        serviceRequest.Id,
                        serviceRequest.Title,
                        Status = serviceRequest.Status.ToString(),
                        CreatedById = serviceRequest.CreatedBy.Id
                    }
                )
            );
        }

        [HttpGet(ApiRoutes.ById)]
        public ActionResult GetById([FromRoute] Guid id) {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty) {
                return Unauthorized();
            }
            
            string? userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if(!Enum.TryParse(userRoleClaim, out UserRole userRole) || !Enum.IsDefined(userRole)){
                return Forbid();
            }

            try {
                var serviceRequest = getServiceRequestByIdUseCase.Execute(
                    id,
                    userId,
                    userRole
                );
                
                if(serviceRequest is null) {
                    return NotFound(new {
                        Error = "A solicitação não foi encontrada."
                    });
                }

                return Ok(new {
                    serviceRequest.Id,
                    serviceRequest.Title,
                    Status = serviceRequest.Status.ToString(),
                    CreatedById = serviceRequest.CreatedBy.Id,
                    CreatedByName = serviceRequest.CreatedBy.Name
                });
            } catch (UnauthorizedAccessException) {
                return Forbid();
            }
        }
    }
}
