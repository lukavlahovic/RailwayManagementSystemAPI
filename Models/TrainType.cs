using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Models
{
    public enum TypeOfTrain
    {
        Passenger,
        Freight,
        HighSpeed,
        Commuter
    }

    public class TrainType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int MaxSpeed { get; set; }

        public int Capacity { get; set; }

        [MaxLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        public TypeOfTrain Type { get; set; }
    }
}
