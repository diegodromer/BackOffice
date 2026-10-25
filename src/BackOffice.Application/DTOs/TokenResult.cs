using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.DTOs {
    public class TokenResult {
        public string AccessToken { get; set; } = string.Empty;

        public DateTimeOffset ExpiresAt { get; set; }
    }
}
