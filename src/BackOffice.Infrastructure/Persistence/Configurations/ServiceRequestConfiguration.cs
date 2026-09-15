using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Infrastructure.Persistence.Configurations {
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest> {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder) {
            builder.ToTable("ServiceRequests");

            builder.HasKey(serviceRequest => serviceRequest.Id);

            builder.Property(serviceRequest => serviceRequest.Id).ValueGeneratedNever();

            builder.Property(serviceRequest => serviceRequest.Title).IsRequired();

            builder.Property(serviceRequest => serviceRequest.Status).HasConversion<string>().IsRequired();

            builder.Property(serviceRequest => serviceRequest.CancellationReason).IsRequired(false);

            builder.Property(serviceRequest =>serviceRequest.ReopeningReason).IsRequired(false);

            builder.HasOne(serviceRequest => serviceRequest.CreatedBy).WithMany().HasForeignKey("CreatedById").IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(serviceRequest => serviceRequest.AssignedTo).WithMany().HasForeignKey("AssignedToId").IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
