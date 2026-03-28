using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Dtos
{
    public class TrainTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxSpeed { get; set; }
        public int Capacity { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public TypeOfTrain TypeOfTrain { get; set; }
    }
}
