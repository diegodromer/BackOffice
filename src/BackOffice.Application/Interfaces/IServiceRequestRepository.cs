using BackOffice.Domain.Entities;

namespace BackOffice.Application.Interfaces {
    public interface IServiceRequestRepository {
        void Add(ServiceRequest serviceRequest);
        
        IReadOnlyList<ServiceRequest> GetAll();

        IReadOnlyList<ServiceRequest> GetByCreatedByUserId(Guid userId);

        ServiceRequest? GetById(Guid serviceRequestId);


    }
}