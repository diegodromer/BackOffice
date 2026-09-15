using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackOffice.Infrastructure.Persistence.Configurations {
    public class UserConfiguration : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.ToTable("Users");
                      
            builder.HasKey(user => user.Id);

            builder.Property(user => user.Id).ValueGeneratedNever();

            builder.Property(user => user.Name).IsRequired();

            builder.Property(user => user.Email).IsRequired();

            builder.Property(user => user.Role).HasConversion<string>().IsRequired();

            builder.Property(user => user.PasswordHash).IsRequired();
        }
    }
}
