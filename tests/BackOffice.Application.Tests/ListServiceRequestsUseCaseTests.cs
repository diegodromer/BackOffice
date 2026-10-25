using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.Tests {
    public class ListServiceRequestsUseCaseTests {

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
                return serviceRequests.FirstOrDefault(
                    ServiceRequest => ServiceRequest.Id == serviceRequestId
                );
            }
        }




        [Theory]
        [InlineData(UserRole.Admin)]
        [InlineData(UserRole.Attendant)]
        public void Execute_WhenRequestsExist_ShouldReturnAllServiceRequests(UserRole userRole) {
            User createdBy = new User(
                "Diego",
                "diego.dromer@estudoscsharpe.com"
            );

            User otherUser = new User(
                "Ana",
                "ana@backoffice.local"
            );

            ServiceRequest firstServiceRequest = new ServiceRequest(
                "Troca de monitor",
                createdBy
            );

            ServiceRequest secondServiceRequest = new ServiceRequest(
                "Instalação de impressora",
                otherUser
            );

            FakeServiceRequestRepository serviceRequestRepository = new FakeServiceRequestRepository();

            serviceRequestRepository.Add(firstServiceRequest);
            serviceRequestRepository.Add(secondServiceRequest);

            ListServiceRequestsUseCase useCase = new ListServiceRequestsUseCase(serviceRequestRepository);

            IReadOnlyList<ServiceRequest> serviceRequests = useCase.Execute(
                createdBy.Id,
                userRole
            );

            Assert.Equal(2, serviceRequests.Count);
            Assert.Same(firstServiceRequest, serviceRequests[0]);
            Assert.Same(secondServiceRequest, serviceRequests[1]);
        }

        [Fact]
        public void Execute_WhenUserIsRequester_ShouldReturnOnlyOwnServiceRequests() {
            User requester = new User(
                "Diego",
                "diego.dromer@estudoscsharpe.com"
            );

            User otherUser = new User(
                "Ana",
                "ana@backoffice.local"
            );

            ServiceRequest ownRequest = new ServiceRequest(
                "Trocar o monitor",
                requester
            );

            ServiceRequest otherRequest = new ServiceRequest(
                "Instalação de impressora",
                otherUser
            );

            FakeServiceRequestRepository repository = new FakeServiceRequestRepository();
            repository.Add(ownRequest);
            repository.Add(otherRequest);

            ListServiceRequestsUseCase useCase = new ListServiceRequestsUseCase(repository);

            IReadOnlyList<ServiceRequest> serviceRequests = useCase.Execute(
                requester.Id, 
                UserRole.Requester
            );

            Assert.Single(serviceRequests);
            Assert.Same(ownRequest, serviceRequests[0]);
        }
    }
}
