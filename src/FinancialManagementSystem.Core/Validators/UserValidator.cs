﻿using FinancialManagementSystem.Core.Entities;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Validators
{
    [ExcludeFromCodeCoverage]
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(user => user.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(user => user.PasswordHash).NotEmpty().WithMessage("PasswordHash is required.");
            RuleFor(user => user.CreatedAt).NotEmpty().WithMessage("CreatedAt is required.");
            RuleFor(user => user.UpdatedAt).NotEmpty().WithMessage("UpdatedAt is required.");
        }
    }
}
