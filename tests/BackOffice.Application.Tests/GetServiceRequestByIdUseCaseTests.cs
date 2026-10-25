using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.Tests {
    public class GetServiceRequestByIdUseCaseTests {

        private class FakeServiceRequestRepository : IServiceRequestRepository {

            private readonly List<ServiceRequest> serviceRequests;

            public FakeServiceRequestRepository() {
                serviceRequests = new List<ServiceRequest>();
            }

            public void Add(ServiceRequest serviceRequest) {
                serviceRequests.Add(serviceRequest);
            }

            public IReadOnlyList<ServiceRequest> GetAll() {
                return serviceRequests.AsReadOnly();
            }

            public IReadOnlyList<ServiceRequest> GetByCreatedByUserId(Guid userId) {
                return serviceRequests
                    .Where(serviceRequest => serviceRequest.CreatedBy.Id == userId)
                    .ToList();
            }

            public ServiceRequest? GetById(Guid serviceRequestId) {
                return serviceRequests.FirstOrDefault(serviceRequest => serviceRequest.Id == serviceRequestId);
            }
        }

        [Theory]
        [InlineData(UserRole.Admin, false)]
        [InlineData(UserRole.Attendant, false)]
        [InlineData(UserRole.Requester, true)]
        public void Execute_WhenUserHasPermission_ShouldReturnServiceRequest(UserRole userRole, bool isOwner) {
            User createBy = new User("Ana", "ana@backoffice.local");
            ServiceRequest serviceRequest = new ServiceRequest("Troca de monitor", createBy);

            FakeServiceRequestRepository repository = new FakeServiceRequestRepository();

            repository.Add(serviceRequest);

            GetServiceRequestByIdUseCase useCase = new GetServiceRequestByIdUseCase(repository);

            Guid authenticatedUserId = isOwner ? createBy.Id : Guid.NewGuid();

            ServiceRequest? result = useCase.Execute(
                serviceRequest.Id,
                authenticatedUserId,
                userRole
            );

            Assert.Same(serviceRequest, result);
        }

        [Fact]
        public void Execute_WhenServiceRequestDoesNotExist_ShouldReturnNull() {
            FakeServiceRequestRepository repository = new FakeServiceRequestRepository();

            GetServiceRequestByIdUseCase useCase = new GetServiceRequestByIdUseCase(repository);

            ServiceRequest? result = useCase.Execute(
                Guid.NewGuid(),
                Guid.NewGuid(),
                UserRole.Admin
            );

            Assert.Null(result);
        }

        [Fact]
        public void Execute_WhenRequesterIsNotOwner_ShouldThrowUnauthorizedAccessException() {
            User createdBy = new User(
                "Ana",
                "ana@backoffice.local"
            );

            ServiceRequest serviceRequest = new ServiceRequest("Troca de monitor", createdBy);

            FakeServiceRequestRepository repository = new FakeServiceRequestRepository();

            repository.Add(serviceRequest) ;

            GetServiceRequestByIdUseCase useCase = new GetServiceRequestByIdUseCase(repository);

            Assert.Throws<UnauthorizedAccessException>(
                () => useCase.Execute(
                    serviceRequest.Id,
                    Guid.NewGuid(),
                    UserRole.Requester
                )
            );
        }

        [Fact]
        public void Execute_WhenRoleIsInvalid_ShouldIdThrowUnauthorizedAccessException() {
            FakeServiceRequestRepository repository = new FakeServiceRequestRepository();
            GetServiceRequestByIdUseCase useCase = new GetServiceRequestByIdUseCase(repository);

            Assert.Throws<UnauthorizedAccessException>(
                () => useCase.Execute(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    (UserRole)0
                )
            );
        }

    }
}
