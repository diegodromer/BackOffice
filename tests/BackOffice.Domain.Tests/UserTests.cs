using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Domain.Tests {
    public class UserTests {
        [Fact]
        public void CreateUser_WithValidNameAndEmail_ShouldCreateUser() {
            var user = new User (
                "Diego Lima",
                "diego@estudoscsharpe.com"
            );

            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.Equal("Diego Lima", user.Name);
            Assert.Equal("diego@estudoscsharpe.com", user.Email);
        }

        [Fact]
        public void CreateUser_WithNameContainingOnlySpaces_ShouldThrowArgumentException() {
            var exception = Assert.Throws<ArgumentException>(() => {
                new User(" ", "diego@estudoscsharpe.com");
            });

            Assert.Equal("name", exception.ParamName);
        }

        [Fact]
        public void CreateUser_WithEmailContainingOnlySpaces_ShouldThrowArgumentException() {
            var exception = Assert.Throws<ArgumentException>(() => {
                new User("Diego Lima", " ");
            });
            Assert.Equal("email", exception.ParamName);
        }

        [Fact]
        public void CreateUser_WithNullName_ShouldThrowArgumentNullException() {
            Assert.Throws<ArgumentNullException>( () => {
                new User(null, "diego.estudoscsharpe.com");
            });
        }

        [Fact]
        public void CreateUser_WithNullEmail_ShouldThrowArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => {
                new User("Diego Lima", null);
            });
        }

        [Fact]
        public void CreateUser_WithSpacesAroundNameAndEmail_ShouldTrimValues() {
            var user = new User(
                " Diego Lima  ", 
                " diego.dromer@estudoscsharpe.com ");
            
            Assert.Equal("Diego Lima", user.Name);
            Assert.Equal("diego.dromer@estudoscsharpe.com", user.Email);
        
        }

        [Fact]
        public void CreateUser_WithoutRole_ShouldCreateRequesterUser() {
            var user = new User(
                "Diego Lima",
                "diego@estudoscsharpe.com"
            );

            Assert.Equal(UserRole.Requester, user.Role);
        }

        [Fact]
        public void CreateUser_WithAttendantRole_ShouldCreateAttendantUser() {
            var user = new User (
                "Diego Lima",
                "diego@estudoscsharpe.com",
                UserRole.Attendant
            );
            Assert.Equal(UserRole.Attendant, user.Role);
        }

        [Fact]
        public void CreateUser_WithInvalidRole_ShouldThrowArgumentException() {
            var exception = Assert.Throws<ArgumentException>(() => {
                new User(
                    "Diego Lima",
                    "diego@estudoscsharpe.com",
                    (UserRole)999
                );
            });
            Assert.Equal("role", exception.ParamName);
        }

        [Fact]
        public void ChangeRole_WithValidRole_ShouldChangeUserRole() {
            var user = new User(
                "Diego Lima",
                "diego@estudoscsharpe.com"
            );
            user.ChangeRole(UserRole.Admin);
            Assert.Equal (UserRole.Admin, user.Role);
        }

        [Fact]
        public void ChangeRole_WithInvalidRole_ShouldThrowArgumentExceptionAndKeepCurrentRole() {
            var user = new User(
                "Diego Lima",
                "diego@estudoscsharpe.com",
                UserRole.Requester
            );
            var exception = Assert.Throws<ArgumentException>(() => {
                user.ChangeRole((UserRole)999);
            });

            Assert.Equal("role", exception.ParamName);
            Assert.Equal(UserRole.Requester, user.Role);
        }

        [Fact]
        public void CreateUser_WithPasswordHash_ShouldCreateUserWithPasswordHash() {
            var user = new User(
                "Diego Lima",
                "diego@estudoscsharpe.com",
                UserRole.Admin,
                "HashTecnico123"
            );

            Assert.Equal("HashTecnico123", user.PasswordHash);
        }

        [Fact]
        public void CreateUser_WithEmptyPasswordHash_ShouldThrowArgumentException() {
            var exception = Assert.Throws<ArgumentException>(() => {
                new User(
                    "Diego Lima",
                    "diego@estudoscsharpe.com",
                    UserRole.Admin,
                    " "
                );
            });

            Assert.Equal("passwordHash", exception.ParamName);
        }

        [Fact]
        public void CreateUser_WithNullPasswordHash_ShouldThrowArgumentNullException() {
            Assert.Throws<ArgumentNullException>( () => {
                new User(
                    "Diego Lima",
                    "diego@estudoscsharpe.com",
                    UserRole.Admin,
                    null!
                );
            });
        }

        [Fact]
        public void ChangePasswordHash_WithValidHash_ShouldChangePasswordHash() {
            var user = new User(
                "Diego Lima",
                "diego@estudoscsharpe.com",
                UserRole.Admin,
                "OldHash"
            );
            user.ChangePasswordHash(" NewHash ");

            Assert.Equal("NewHash", user.PasswordHash);
        }

        [Fact]
        public void ChangePasswordHash_WithEmptyHash_ShouldThrowArgumentExceptionAndKeepCurrentHash() {
            var user = new User(
                "Diego Lima",
                "diego@estudoscsharpe.com",
                UserRole.Admin,
                "CurrentHash"
            );

            var exception = Assert.Throws<ArgumentException>(() => {
                user.ChangePasswordHash(" ");
            });

            Assert.Equal("passwordHash", exception.ParamName);
            Assert.Equal("CurrentHash", user.PasswordHash);
        }
    }
}
