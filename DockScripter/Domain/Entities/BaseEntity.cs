namespace ParkOnyx.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } 

    [Required] public DateTime CreationDateTimeUtc { get; set; } = DateTime.UtcNow; 

    [Required] public DateTime LastUpdatedDateTimeUtc { get; set; } = DateTime.UtcNow; 
}