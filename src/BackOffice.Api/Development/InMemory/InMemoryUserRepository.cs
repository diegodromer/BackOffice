using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;

namespace BackOffice.Api.Development.InMemory {
    public class InMemoryUserRepository : IUserRepository {
        private readonly List<User> users;

        public User DevelopmentUser { get; }
        
        public InMemoryUserRepository() {
            DevelopmentUser = new User(
                "Usuário de Desenvolvimento",
                "desenvolvimento@backoffice.local"
            );
            users = new List<User> {
                DevelopmentUser
            };
        }

        public User? GetById(Guid userId) {
            foreach(User user in users) {
                if(user.Id == userId) {
                    return user;
                }
            }
            return null;
        }

        public User? GetByEmail(string email) {
            return DevelopmentUser.Email == email ? DevelopmentUser : null;
        }
    }
}
