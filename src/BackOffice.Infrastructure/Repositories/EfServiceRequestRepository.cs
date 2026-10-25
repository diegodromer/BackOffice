using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace BackOffice.Infrastructure.Repositories {
    public class EfServiceRequestRepository : IServiceRequestRepository {
        private readonly BackOfficeDbContext dbContext;


        public EfServiceRequestRepository(BackOfficeDbContext context) {
            this.dbContext = context;
        }

        public void Add(ServiceRequest serviceRequest) {
            dbContext.ServiceRequests.Add(serviceRequest);
            dbContext.SaveChanges();
        }

        public IReadOnlyList<ServiceRequest> GetAll() {
            return dbContext.ServiceRequests
                .Include(serviceRequest => serviceRequest.CreatedBy)
                .Include(serviceRequest => serviceRequest.AssignedTo)
                .ToList();
        }

        public IReadOnlyList<ServiceRequest> GetByCreatedByUserId(Guid userId) {
            return dbContext.ServiceRequests
                .Where(serviceRequest => serviceRequest.CreatedBy.Id == userId)
                .Include(serviceRequest => serviceRequest.CreatedBy)
                .Include(serviceRequest => serviceRequest.AssignedTo)
                .ToList();
        }

        public ServiceRequest? GetById(Guid serviceRequestId) {
            return dbContext.ServiceRequests
                .Include(serviceRequest => serviceRequest.CreatedBy)
                .Include(serviceRequest => serviceRequest.AssignedTo)
                .FirstOrDefault(serviceRequest => serviceRequest.Id == serviceRequestId);
        }
    }
}
