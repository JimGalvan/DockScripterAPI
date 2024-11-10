using System.ComponentModel.DataAnnotations;

namespace DockScripter.Domain.Dtos.Requests;

public class EnvironmentRequestDto
{
    [Required] public string EnvironmentName { get; set; }
}