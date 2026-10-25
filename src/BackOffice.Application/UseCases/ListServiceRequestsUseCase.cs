using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;

namespace BackOffice.Application.UseCases {
    public class ListServiceRequestsUseCase {
        private readonly IServiceRequestRepository serviceRequestRepository;

        public ListServiceRequestsUseCase(
            IServiceRequestRepository serviceRequestRepository
        ) {
            this.serviceRequestRepository = serviceRequestRepository;
        }

        public IReadOnlyList<ServiceRequest> Execute(Guid userId, UserRole userRole) {
            switch (userRole) {
                case UserRole.Admin:
                case UserRole.Attendant:
                    return serviceRequestRepository.GetAll();

                case UserRole.Requester:
                    return serviceRequestRepository.GetByCreatedByUserId(userId);

                default:
                    throw new UnauthorizedAccessException(
                        "O perfil do usuário não tem permissão para consultar solicitações."
                    );
            }
        }
    }
}
