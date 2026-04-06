using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateDelayDto
    {
        [Required]
        public int TripId { get; set; }

        [Required]
        public int StationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int DelayMinutes { get; set; }

        public TypeOfDelay TypeOfDelay { get; set; }

        [Length(0, 250, ErrorMessage = "Length of note is maximum 250 characters")]
        public string Note { get; set; } = string.Empty;
    }
}
