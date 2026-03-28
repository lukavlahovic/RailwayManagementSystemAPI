using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateTrainDto
    {
        [Required]
        public int TrainTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SerialNumber { get; set; } = string.Empty;
    }
}
