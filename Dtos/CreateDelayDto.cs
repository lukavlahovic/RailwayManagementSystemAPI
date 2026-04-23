using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateDelayDto
    {
        public int TripId { get; set; }

        public int StationId { get; set; }

        public int DelayMinutes { get; set; }

        public TypeOfDelay TypeOfDelay { get; set; }

        [Length(0, 250, ErrorMessage = "Length of note is maximum 250 characters")]
        public string Note { get; set; } = string.Empty;
    }
}
