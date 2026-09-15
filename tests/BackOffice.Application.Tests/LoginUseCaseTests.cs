using BackOffice.Application.DTOs;
using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BackOffice.Application.Tests {
    public class LoginUseCaseTests {

        [Fact]
        public void Execute_WhenCredentialsAreValid_ShouldReturnLoginResult() {
            User user = new User(
                "Diego",
                "diego.dromer@estudoscsharpe.com",
                UserRole.Admin,
                "StoredPasswordHash"
            );

            FakeUserRepository userRepository = new FakeUserRepository(user);
            FakePasswordHasher passwordHasher = new FakePasswordHasher(true);

            LoginUseCase useCase = new LoginUseCase(userRepository, passwordHasher);

            LoginInput input = new LoginInput {
                Email = "diego.dromer@estudoscsharpe.com",
                Password = "123456"
            };

            LoginResult result = useCase.Execute(input);

            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Role, result.Role);
        }

        private class FakeUserRepository : IUserRepository {
            private readonly User? user;

            public FakeUserRepository(User? user) {
                this.user = user;
            }

            public User? GetById(Guid userId) {
                if (user?.Id == userId) {
                    return user;
                }

                return null;
            }

            public User? GetByEmail(string email) {
                if(user?.Email == email) {
                    return user;
                }
                return null;
            }
        }

        private class FakePasswordHasher : IPasswordHasher { 
            private readonly bool verificationResult;

            public FakePasswordHasher(bool verificationResult) {
                this.verificationResult = verificationResult;
            }

            public string Hash(string password) {
                return "FakePasswordHash";
            }

            public bool Verify(string password, string passwordHash) {
                return verificationResult;
            }
        }

        [Fact]
        public void Execute_WhenUserDoesNotExist_ShouldThrowInvalidOperationException() {
            FakeUserRepository userRepository = new FakeUserRepository(null);
            FakePasswordHasher passwordHasher = new FakePasswordHasher(true);

            LoginUseCase useCase = new LoginUseCase(
                userRepository,
                passwordHasher
            );

            LoginInput input = new LoginInput {
                Email = "naoexiste@estudoscsharpe.com",
                Password = "123456"
            };

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => useCase.Execute(input));

            Assert.Equal("E-mail ou senha inválidos.", exception.Message);
        }

        [Fact]
        public void Execute_WhenPasswordIsInvalid_ShouldThrowInvalidOperationException() {
            User user = new User(
                "Diego",
                "diego.dromer@estudoscsharpe.com",
                UserRole.Admin,
                "StoredPasswordHash"
            );

            FakeUserRepository userRepository = new FakeUserRepository(user);
            FakePasswordHasher passwordHasher = new FakePasswordHasher(false);

            LoginUseCase useCase = new LoginUseCase(
                userRepository,
                passwordHasher
            );

            LoginInput input = new LoginInput {
                Email = "diego.dromer@estudoscsharpe.com",
                Password = "senha-errada"
            };    

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => useCase.Execute(input)
            );

            Assert.Equal("E-mail ou senha inválidos.", exception.Message);
        }

        [Fact]
        public void Execute_WhenEmailIsBlank_ShouldThrowArgumentException() {
            FakeUserRepository userRepository = new FakeUserRepository(null);
            FakePasswordHasher passwordHasher = new FakePasswordHasher(true);

            LoginUseCase useCase = new LoginUseCase (
                userRepository, 
                passwordHasher
            );

            LoginInput input = new LoginInput {
                Email = " ",
                Password = "123456"
            };

            ArgumentException exception = Assert.Throws<ArgumentException>(() => 
                useCase.Execute(input)
            );

            Assert.Contains("O e-mail é obrigatório.", exception.Message);
            Assert.Equal("input", exception.ParamName);
        }

        [Fact]
        public void Execute_WhenPasswordIsBlank_ShouldThrowArgumentException() {
            FakeUserRepository userRepository = new FakeUserRepository(null);
            FakePasswordHasher passwordHasher = new FakePasswordHasher(true);

            LoginUseCase useCase = new LoginUseCase(
                userRepository,
                passwordHasher
            );

            LoginInput input = new LoginInput {
                Email = "diego.dromer@estudoscharpe.com",
                Password = " "
            };

            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                useCase.Execute(input)
            );

            Assert.Contains("A senha é obrigatória.", exception.Message);
            Assert.Equal("input", exception.ParamName);
        }
    }
}
