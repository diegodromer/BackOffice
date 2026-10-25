using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;

namespace BackOffice.Infrastructure.Persistence {
    public static  class BackOfficeDatabaseSeeder {
        public static User SeedDevelopmentUser(BackOfficeDbContext dbContext, IPasswordHasher passwordHasher) {
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(passwordHasher);

            const string developmentUserEmail = "diego.dromer@estudoscsharpe.com";
            const string developmentPassword = "Senha123!";

            User? developmentUser = dbContext.Users.FirstOrDefault(
                user => user.Email == developmentUserEmail
            );

            if(developmentUser is not null) {
                bool hasChanges = false;

                if(developmentUser.Role != UserRole.Admin) {
                    developmentUser.ChangeRole(UserRole.Admin);
                    hasChanges = true;
                }

                if(developmentUser.PasswordHash == "TemporaryPasswordHash" || developmentUser.PasswordHash == "DevelopmentPasswordHash") {
                    string passwordHash = passwordHasher.Hash(developmentPassword);

                    developmentUser.ChangePasswordHash(passwordHash);
                    hasChanges = true;
                }

                if (hasChanges) {
                    dbContext.SaveChanges();
                }

                return developmentUser;
            }

            developmentUser = new User(
                "Diego",
                developmentUserEmail,
                UserRole.Admin,
                passwordHasher.Hash(developmentPassword)
            );

            dbContext.Users.Add(developmentUser);
            dbContext.SaveChanges();

            return developmentUser;
        }
    }
}
