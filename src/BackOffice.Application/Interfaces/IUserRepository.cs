using BackOffice.Domain.Entities;

namespace BackOffice.Application.Interfaces {
    public interface IUserRepository {
        User? GetById(Guid userId);
        User? GetByEmail(string email);
    }
}
