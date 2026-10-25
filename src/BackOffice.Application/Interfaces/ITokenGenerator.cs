using BackOffice.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.Interfaces {
    public interface ITokenGenerator {
        TokenResult Generate(LoginResult loginResult);
    }
}
