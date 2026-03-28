using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateTrainTypeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 500)]
        public int MaxSpeed { get; set; }

        [Range(1, 2000)]
        public int Capacity { get; set; }

        [EnumDataType(typeof(TrainType))]

        [MaxLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        public TrainType Type { get; set; } = null!;
    }
}
