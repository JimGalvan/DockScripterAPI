using DockScripter.Domain.Entities;
using DockScripter.Entities;

namespace DockScripter.Domain.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ReservationEntity : BaseEntity
{
    [Required] public DateTime StartTime { get; set; } // Reservation start time

    [Required] public DateTime EndTime { get; set; } // Reservation end time

    [Required] public bool IsPaid { get; set; } // Whether the reservation has been paid

    // Foreign Keys
    [Required] public Guid UserId { get; set; }
    [ForeignKey("UserId")] public UserEntity UserEntity { get; set; }

    [Required] public Guid ParkingSpotId { get; set; }
    [ForeignKey("ParkingSpotId")] public ParkingSpotEntity SpotEntity { get; set; }
}