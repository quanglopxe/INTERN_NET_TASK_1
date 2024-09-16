﻿using MilkStore.Core.Base;

namespace MilkStore.Contract.Repositories.Entity
{
    public class UserInfo : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string? BankAccount { get; set; }
        public string? BankAccountName { get; set; }
        public string? Bank { get; set; }
    }
}