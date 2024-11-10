﻿using DockScripter.Domain.Entities;
using DockScripter.Domain.Enums;

namespace DockScripter.Domain.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class UserEntity : BaseEntity
{
    [Required] [MaxLength(50)] public string? FirstName { get; set; }
    [Required] [MaxLength(50)] public string? LastName { get; set; }

    public byte[]? PasswordHash { get; init; }
    public byte[]? PasswordSalt { get; init; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; init; }

    [Required] public List<UserRole> Roles { get; init; } = new();

    // Relationships
}