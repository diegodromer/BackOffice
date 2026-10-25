using BackOffice.Application.DTOs;
using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;

namespace BackOffice.Application.Tests {
    public class CreateServiceRequestUseCaseTests {

        private class FakeUserRepository : IUserRepository {
            private readonly User? user;

            public FakeUserRepository(User? user) {
                this.user = user;
            }

            public User? GetByEmail(string email) {
                return null;
            }

            public User? GetById(Guid userId) {
                if (user is null || user.Id != userId) {
                    return null;
                }
                return user;
            }
        }
        private class FakeServiceRequestRepository : IServiceRequestRepository {
            private readonly List<ServiceRequest> serviceRequests;

            public ServiceRequest? AddedServiceRequest { get; private set; }

            public FakeServiceRequestRepository() {
                serviceRequests = new List<ServiceRequest>();
            }

            public void Add(ServiceRequest serviceRequest) {
                AddedServiceRequest = serviceRequest;
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
                return serviceRequests.FirstOrDefault(
                    ServiceRequest => ServiceRequest.Id == serviceRequestId
                );
            }
        }

        [Theory]
        [InlineData(UserRole.Admin)]
        [InlineData(UserRole.Attendant)]
        [InlineData(UserRole.Requester)]
        public void Execute_WhenUserExists_ShouldCreateAndSaveServiceRequest(UserRole userRole) {
            User createdBy = new User(
                "Diego",
                "diego.dromer@estudoscsharpe.com"
            );

            FakeUserRepository userRepository = new FakeUserRepository(createdBy);

            FakeServiceRequestRepository serviceRequestRepository = new FakeServiceRequestRepository();

            CreateServiceRequestUseCase useCase = new CreateServiceRequestUseCase(userRepository, serviceRequestRepository);

            CreateServiceRequestInput input = new CreateServiceRequestInput(" Solicitação de teste", createdBy.Id);

            ServiceRequest serviceRequest = useCase.Execute(
                input, 
                createdBy.Id, 
                userRole
            );

            Assert.Equal("Solicitação de teste", serviceRequest.Title);
            Assert.Same(createdBy, serviceRequest.CreatedBy);
            Assert.Same(
                serviceRequest,
                serviceRequestRepository.AddedServiceRequest
            );
        }

        [Fact]
        public void Execute_WhenUserDoesNotExist_ShouldThrowInvalidOperationException() {
            FakeUserRepository userRepository = new FakeUserRepository(null);

            FakeServiceRequestRepository serviceRequestRepository = new FakeServiceRequestRepository();

            CreateServiceRequestUseCase useCase = new CreateServiceRequestUseCase(
                userRepository,
                serviceRequestRepository
            );

            CreateServiceRequestInput input = new CreateServiceRequestInput(
                "Solicitação de teste",
                Guid.NewGuid()
            );

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => useCase.Execute(input, Guid.NewGuid(), UserRole.Admin)
            );

            Assert.Null(serviceRequestRepository.AddedServiceRequest);
        }

        [Fact]
        public void Execute_WhenRequesterCreatesForAnotherUser_ShouldThrowUnauthorizedAccessException() {
            User requester = new User(
                "Diego",
                "diego.dromer@estudoscsharpe.com"
            );

            User otherUser = new User(
                "Ana",
                "ana@backoffice.loca"
            );

            FakeUserRepository userRepository = new FakeUserRepository(otherUser);
            FakeServiceRequestRepository serviceRequestRepository = new FakeServiceRequestRepository();

            CreateServiceRequestUseCase useCase = new CreateServiceRequestUseCase(userRepository, serviceRequestRepository);

            CreateServiceRequestInput input = new CreateServiceRequestInput("Solicitação em nome de outra pessoa", otherUser.Id);

            Assert.Throws<UnauthorizedAccessException>(
                () => useCase.Execute(
                    input,
                    requester.Id,
                    UserRole.Requester
                )
            );

            Assert.Null(serviceRequestRepository.AddedServiceRequest);
        }


    }
}
