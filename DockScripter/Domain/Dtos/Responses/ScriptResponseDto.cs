﻿namespace DockScripter.Domain.Dtos.Responses;

public class ScriptResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public string Status { get; set; }
    public DateTime CreationDateTimeUtc { get; set; }
    public DateTime? LastExecutedAt { get; set; }
}