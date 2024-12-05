﻿using Microsoft.AspNetCore.Identity;

namespace AccessManagementSystem_API.Models
{
    public class AppUser: IdentityUser
    {
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
