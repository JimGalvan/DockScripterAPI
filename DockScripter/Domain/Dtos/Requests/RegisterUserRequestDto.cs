using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper.Configuration.Annotations;
using DockScripter.Domain.Enums;
using DockScripter.Domain.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace DockScripter.Domain.Dtos.Requests
{
    public class RegisterUserRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; }
    }
}