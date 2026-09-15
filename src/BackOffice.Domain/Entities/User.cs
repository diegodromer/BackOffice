using BackOffice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Domain.Entities {
    public class User {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public UserRole Role { get; private set; }

        public string PasswordHash { get; private set;}

        public User(string name, string email, UserRole role = UserRole.Requester, string passwordHash = "TemporaryPasswordHash") {

            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(email);

            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("O nome do usuário é obrigatório", nameof(name));
            }

            if(string.IsNullOrWhiteSpace(email)) {
                throw new ArgumentException("O e-mail do usuário é obrigatório", nameof(email));
            }

            if (!Enum.IsDefined(role)) {
                throw new ArgumentException("O perfil do usuário é inválido.", nameof(role));
            }

            ArgumentNullException.ThrowIfNull(passwordHash);
            if (string.IsNullOrWhiteSpace(passwordHash)) { 
                throw new ArgumentException("O hash da senha do usuário é obrigatório.", nameof(passwordHash));
            }

            Id = Guid.NewGuid();
            Name = name.Trim();
            Email = email.Trim();
            Role = role;
            PasswordHash = passwordHash.Trim();
        }

        public void ChangeRole(UserRole role) {
            if (!Enum.IsDefined(role)) {
                throw new ArgumentException("O perfil do usuário é inválido.", nameof(role));
            }

            Role = role;
        }

        public void ChangePasswordHash(string passwordHash) {
            ArgumentNullException.ThrowIfNull(passwordHash);

            if (string.IsNullOrWhiteSpace(passwordHash)) {
                throw new ArgumentException("O hash da senha do usuário é obrigatório.", nameof(passwordHash));
            }

            PasswordHash = passwordHash.Trim();
        }
    }
}
