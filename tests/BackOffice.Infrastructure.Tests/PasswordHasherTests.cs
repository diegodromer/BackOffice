using BackOffice.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Infrastructure.Tests {
    public class PasswordHasherTests {

        [Fact]
        public void Hash_WhenPasswordIsValid_ShouldReturnHashDifferentFromPassord() {
            PasswordHasher passwordHasher = new PasswordHasher();

            string password = "Senha123!";

            string passwordHash = passwordHasher.Hash(password);

            Assert.NotEqual(password, passwordHash);
            Assert.StartsWith("PBKDF2$", passwordHash);
        }

        [Fact]
        public void Verify_WhenPasswordIsCorrect_ShouldReturnTrue() {
            PasswordHasher passwordHasher = new PasswordHasher();

            string password = "Senha123!";
            string passwordHash = passwordHasher.Hash(password);

            bool result = passwordHasher.Verify(
                password,
                passwordHash
            );

            Assert.True(result);
        }

        [Fact]
        public void Verify_WhenPasswordIsIncorrect_ShouldReturnFalse() {
            PasswordHasher passwordHasher = new PasswordHasher();

            string password = "Senha123!";
            string passwordHash = passwordHasher.Hash(password);

            bool result = passwordHasher.Verify(
                "SenhaErrada123!",
                passwordHash
            );

            Assert.False(result);
        }

        [Fact]
        public void Hash_WhenPasswordIsSame_ShouldReturnDifferentHashes() {
            PasswordHasher passwordHasher = new PasswordHasher();

            string password = "Senha123!";

            string firstHash = passwordHasher.Hash(password);
            string secondHash = passwordHasher.Hash(password);

            Assert.NotEqual(firstHash, secondHash);
            Assert.True(passwordHasher.Verify(password, firstHash));
            Assert.True(passwordHasher.Verify(password, secondHash));

        }

        [Fact]
        public void Hash_WhenPasswordIsEmpty_ShouldThrowArgumentException() {
            PasswordHasher passwordHasher = new PasswordHasher();

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => passwordHasher.Hash(string.Empty)
            );

            Assert.Equal("password", exception.ParamName);
        }

        [Fact]
        public void Hash_WhenPasswordIsNull_ShouldThrowsArgumentNullException() {
            PasswordHasher passwordHasher = new PasswordHasher();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => passwordHasher.Hash(null!));

            Assert.Equal("password", exception.ParamName);
        }

        [Fact]
        public void Verify_WhenPasswordIsEmpty_ShouldReturnFalse() {
            PasswordHasher passwordHasher = new PasswordHasher();

            string passwordHash = passwordHasher.Hash("Senha123!");

            bool result = passwordHasher.Verify(string.Empty, passwordHash);

            Assert.False(result);
        }

        [Fact]
        public void Verify_WhenPasswordHashFormatIsInvalid_ShouldReturnFalse() {
            PasswordHasher passwordHasher = new PasswordHasher();

            string password = "Senha123!";
            string passwordHash = "InvalidHash";

            bool result = passwordHasher.Verify(password, passwordHash);

            Assert.False(result);
        }

        [Fact]
        public void Verify_WhenPasswordHashContainsInvalidBase64_ShouldReturnFalse() {
            PasswordHasher passwordHasher = new PasswordHasher();

            string password = "Senha123!";
            string passwordHash = "PBKDF2$100000$InvalidBase64!$InvalidBase64!";

            bool result = passwordHasher.Verify(password, passwordHash);

            Assert.False(result);
        }
    }
 }