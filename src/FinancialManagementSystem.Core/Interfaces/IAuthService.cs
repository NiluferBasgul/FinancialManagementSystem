﻿using FinancialManagementSystem.Core.Models;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginModel model);
        Task<AuthResult> RegisterAsync(RegisterModel model);
        int? ValidateToken(string token);
    }
}