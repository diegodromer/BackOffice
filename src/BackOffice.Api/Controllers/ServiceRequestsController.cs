using BackOffice.Application.DTOs;
using BackOffice.Application.UseCases;
using Microsoft.AspNetCore.Mvc;


namespace BackOffice.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase {
        private readonly CreateServiceRequestUseCase createServiceRequestUseCase;
        private readonly ListServiceRequestsUseCase listServiceRequestsUseCase;

        public ServiceRequestsController(
            CreateServiceRequestUseCase createServiceRequestUseCase,
            ListServiceRequestsUseCase listServiceRequestsUseCase
        ) {
            this.createServiceRequestUseCase = createServiceRequestUseCase;
            this.listServiceRequestsUseCase = listServiceRequestsUseCase;
        }

    [HttpPost]
    public ActionResult Create(
            [FromBody] CreateServiceRequestInput input
        ) {
            try {
                var serviceRequest = createServiceRequestUseCase.Execute(input);

                return Created(
                    $"/api/ServiceRequests/{serviceRequest.Id}",
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
            } catch(InvalidOperationException exception) { 
                return BadRequest(new {
                    Error = exception.Message
                });
            }
        }

    [HttpGet]
    public ActionResult Get() {
        var serviceRequests = listServiceRequestsUseCase.Execute();
            return Ok (
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
    }
}
