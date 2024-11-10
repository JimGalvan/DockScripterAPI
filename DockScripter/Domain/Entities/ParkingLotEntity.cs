namespace ParkOnyx.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ParkingLotEntity : BaseEntity
{
    [Required] [MaxLength(100)] public string Name { get; set; } // Name of the parking lot, Max Length: 100

    [Required] [MaxLength(255)] public string Location { get; set; } // Address, Max Length: 255

    [Required] public double Latitude { get; set; } // Latitude coordinate

    [Required] public double Longitude { get; set; } // Longitude coordinate

    [Required] public int TotalSpots { get; set; } // Total number of parking spots

    [Required] public int AvailableSpots { get; set; } // Number of available parking spots

    [MaxLength(500)] public string Description { get; set; } // Description of amenities, Max Length: 500

    // Foreign Key
    [Required] public Guid OwnerId { get; set; }
    [ForeignKey("OwnerId")] public UserEntity Owner { get; set; }

    // Relationships
    public ICollection<ParkingSpotEntity> ParkingSpots { get; set; }
}