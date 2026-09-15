using BackOffice.Application.DTOs;
using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.UseCases {
    public class CreateServiceRequestUseCase {
        private readonly IUserRepository userRepository;
        private readonly IServiceRequestRepository serviceRequestRepository;

        public CreateServiceRequestUseCase(
            IUserRepository userRepository,
            IServiceRequestRepository serviceRequestRepository
        ) {
            this.userRepository = userRepository;
            this.serviceRequestRepository = serviceRequestRepository;
        }

        public ServiceRequest Execute(CreateServiceRequestInput input) { 
            ArgumentNullException.ThrowIfNull( input );

            User? createBy = userRepository.GetById(input.CreatedByUserId);

            if(createBy is null) {
                throw new InvalidOperationException(
                    "O usuário que abriu a solicitação não foi encontrado."
                    );
            }

            ServiceRequest serviceRequest = new ServiceRequest(
                input.Title,
                createBy
            );

            serviceRequestRepository.Add(serviceRequest);

            return serviceRequest;
        }
    }
}
