using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using BackOffice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.Tests {
    public class ListServiceRequestsUseCaseTests {
        [Fact]
        public void Execute_WhenRequestsExist_ShouldReturnAllServiceRequests() {
            User createdBy = new User(
                "Diego",
                "diego.dromer@estudoscsharpe.com"
            );

            ServiceRequest firstServiceRequest = new ServiceRequest(
                "Troca de monitor",
                createdBy
            );

            ServiceRequest secondServiceRequest = new ServiceRequest(
                "Instalação de impressora",
                createdBy
            );

            FakeServiceRequestRepository serviceRequestRepository = new FakeServiceRequestRepository();

            serviceRequestRepository.Add(firstServiceRequest);
            serviceRequestRepository.Add(secondServiceRequest);

            ListServiceRequestsUseCase useCase = new ListServiceRequestsUseCase(serviceRequestRepository);

            IReadOnlyList<ServiceRequest> serviceRequests = useCase.Execute();

            Assert.Equal(2, serviceRequests.Count);
            Assert.Same(firstServiceRequest, serviceRequests[0]);
            Assert.Same(secondServiceRequest, serviceRequests[1]);
        }

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
        }
    }   
}
