﻿using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem_API.Dtos
{
    public class UserDetailDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; } 
        public string[]? Roles { get; set; }
        public string? PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}
