﻿namespace FinancialManagementSystem.Core.Models
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }
}