using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;
using BackOffice.Infrastructure.Persistence;

namespace BackOffice.Infrastructure.Repositories {
    public class EfUserRepository : IUserRepository {
        private readonly BackOfficeDbContext dbContext;

        public EfUserRepository(BackOfficeDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public User? GetById(Guid userId) {
            return dbContext.Users.Find(userId);
        }

        public User? GetByEmail(string email) {
            return dbContext.Users.FirstOrDefault(user => user.Email == email);
        }
    }
}
