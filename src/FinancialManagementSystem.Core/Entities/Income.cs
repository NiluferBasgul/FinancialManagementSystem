﻿using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Income
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}