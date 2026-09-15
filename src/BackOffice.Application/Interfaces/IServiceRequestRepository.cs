using BackOffice.Domain.Entities;

namespace BackOffice.Application.Interfaces {
    public interface IServiceRequestRepository {
        void Add(ServiceRequest serviceRequest);
        
        IReadOnlyList<ServiceRequest> GetAll();
    }
}