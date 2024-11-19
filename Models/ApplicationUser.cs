﻿using Microsoft.AspNetCore.Identity;

namespace ERP.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FristName { get; set; }
        public string? LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? Status { get; set; }
    }
}