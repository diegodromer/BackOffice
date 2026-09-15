using BackOffice.Application.DTOs;
using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using BackOffice.Domain.Entities;

namespace BackOffice.Application.Tests {
    public class CreateServiceRequestUseCaseTests {
        [Fact]
        public void Execute_WhenUserExists_ShouldCreateAndSaveServiceRequest() {
            User createdBy = new User(
                "Diego",
                "diego.dromer@estudoscsharpe.com"
            );

            FakeUserRepository userRepository = new FakeUserRepository(createdBy);

            FakeServiceRequestRepository serviceRequestRepository = new FakeServiceRequestRepository();

            CreateServiceRequestUseCase useCase = new CreateServiceRequestUseCase(userRepository, serviceRequestRepository);

            CreateServiceRequestInput input = new CreateServiceRequestInput(" Solicitação de teste", createdBy.Id);

            ServiceRequest serviceRequest = useCase.Execute(input);

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
                () => useCase.Execute(input)
            );

            Assert.Null(serviceRequestRepository.AddedServiceRequest);
        }

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
        }
    }
}
