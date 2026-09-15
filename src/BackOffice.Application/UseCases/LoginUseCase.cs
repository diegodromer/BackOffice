using BackOffice.Application.DTOs;
using BackOffice.Application.Interfaces;
using BackOffice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.UseCases {
    public class LoginUseCase {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;

        public LoginUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher
        ) {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public LoginResult Execute(LoginInput input) {
           ArgumentNullException.ThrowIfNull(input);

            if (string.IsNullOrWhiteSpace(input.Email)) {
                throw new ArgumentException(
                    "O e-mail é obrigatório.",
                    nameof(input)
                );
            }

            if (string.IsNullOrWhiteSpace(input.Password)) {
                throw new ArgumentException(
                    "A senha é obrigatória.",
                    nameof (input)
                );
            }

            User? user = userRepository.GetByEmail(input.Email.Trim());

            if (user is null) {
                throw new InvalidOperationException("E-mail ou senha inválidos.");
            }

            bool passwordIsValid = passwordHasher.Verify(
                input.Password,
                user.PasswordHash
            );

            if (!passwordIsValid) {
                throw new InvalidOperationException("E-mail ou senha inválidos.");
            }

            return new LoginResult {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
