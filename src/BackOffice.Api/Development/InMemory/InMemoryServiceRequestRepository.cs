using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;
using System.Runtime.CompilerServices;

namespace BackOffice.Api.Development.InMemory {
    public class InMemoryServiceRequestRepository : IServiceRequestRepository {

        private readonly List<ServiceRequest> serviceRequests;

        public InMemoryServiceRequestRepository() {
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
}
