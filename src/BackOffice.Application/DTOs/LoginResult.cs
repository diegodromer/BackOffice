using BackOffice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.DTOs {
    public class LoginResult {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; }
    }
}
