using BackOffice.Application.Interfaces;
using System.Security.Cryptography;

namespace BackOffice.Infrastructure.Security {
    public class PasswordHasher : IPasswordHasher {
        private const int Iterations = 100000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public string Hash(string password) {
            ArgumentNullException.ThrowIfNull(password);

            if (string.IsNullOrWhiteSpace(password)) {
                throw new ArgumentException(
                    "A senha é obrigatória.",
                    nameof(password)
                );
            }

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );

            return string.Join(
                "$",
                "PBKDF2",
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash)
            );
        }

        public bool Verify(string password, string passwordHash) {
            ArgumentNullException.ThrowIfNull(password);
            ArgumentNullException.ThrowIfNull(passwordHash);

            if (string.IsNullOrWhiteSpace(password)) {
                return false;
            }

            string[] parts = passwordHash.Split('$');

            if (parts.Length != 4 || parts[0] != "PBKDF2") {
                return false;
            }

            if (!int.TryParse(parts[1], out int iterations)) {
                return false;
            }

            try {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expectedHash = Convert.FromBase64String(parts[3]);

                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length
                );

                return CryptographicOperations.FixedTimeEquals(
                    actualHash,
                    expectedHash
                );
            } catch (FormatException) {
                return false;
            }
        }
    }
}