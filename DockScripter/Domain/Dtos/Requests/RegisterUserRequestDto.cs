using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper.Configuration.Annotations;
using ParkOnyx.Domain.Enums;
using ParkOnyx.Domain.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace ParkOnyx.Domain.Dtos.Requests
{
    public class RegisterUserRequestDto
    {
        [DefaultValue("John")]
        [Required(ErrorMessage = "First name is required")]
        public string? FirstName { get; set; }

        [DefaultValue("Doe")]
        [Required(ErrorMessage = "Last name is required")]
        public string? LastName { get; set; }

        [DefaultValue("user@example.com")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [DefaultValue("User")]
        [Required(ErrorMessage = "User role is required")]
        public string? Role { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; }
    }
}