using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class TrainDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0,500)]
        public int MaxSpeed { get; set; }

        [Range(0,2000)]
        public int Capacity { get; set; }

        [EnumDataType(typeof(TrainType))]
        public TrainType Type { get; set; }
    }
}
