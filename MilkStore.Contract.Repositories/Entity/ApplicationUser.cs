﻿using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;

namespace MilkStore.Repositories.Entity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Password {  get; set; } = string.Empty;
        public virtual UserInfo? UserInfo { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public ApplicationUser()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
