using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;

namespace BackOffice.Application.UseCases {
    public class ListServiceRequestsUseCase {
        private readonly IServiceRequestRepository serviceRequestRepository;

        public ListServiceRequestsUseCase(
            IServiceRequestRepository serviceRequestRepository
        ) {
            this.serviceRequestRepository = serviceRequestRepository;
        }

        public IReadOnlyList<ServiceRequest> Execute() {
            return serviceRequestRepository.GetAll();
        }
    }
}
