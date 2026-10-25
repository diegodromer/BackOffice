using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.UseCases {
    public class GetServiceRequestByIdUseCase {
        private readonly IServiceRequestRepository serviceRequestRepository;

        public GetServiceRequestByIdUseCase(IServiceRequestRepository serviceRequestRepository) {
            this.serviceRequestRepository = serviceRequestRepository;
        }

        public ServiceRequest? Execute(
            Guid serviceRequestId,
            Guid authenticateUseId,
            UserRole userRole
        ) {
            switch (userRole) {
                case UserRole.Admin:
                case UserRole.Attendant:
                case UserRole.Requester:
                    break;

                default:
                    throw new UnauthorizedAccessException("O perfil do usuário não tem permissão para consultar solicitações");
            }

            ServiceRequest? serviceRequest = serviceRequestRepository.GetById(serviceRequestId);

            if (serviceRequest is null) {
                return null;    
            }

            if(userRole == UserRole.Requester && serviceRequest.CreatedBy.Id != authenticateUseId) {
                throw new UnauthorizedAccessException("Você só pode consultar suas próprias solicitações.");
            }

            return serviceRequest;
        }
    }
}
