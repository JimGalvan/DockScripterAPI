using DockScripter.Entities;

namespace DockScripter.Domain.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ParkingSpotEntity : BaseEntity
{
    [Required] public bool IsReserved { get; init; } // Whether the spot is currently reserved

    [Required] [MaxLength(20)] public string SpotNumber { get; init; } // Identifier for the spot, Max Length: 20

    // Foreign Key
    [Required] public Guid LotId { get; set; }
    [ForeignKey("LotId")] public ParkingLotEntity LotEntity { get; set; }

    // Relationships
    public ICollection<ReservationEntity> Reservations { get; set; }
}