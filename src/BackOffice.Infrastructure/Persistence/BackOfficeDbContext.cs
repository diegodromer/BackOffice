using BackOffice.Domain.Entities;
using BackOffice.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Infrastructure.Persistence {
    public class BackOfficeDbContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public BackOfficeDbContext(DbContextOptions<BackOfficeDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceRequestConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
