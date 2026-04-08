using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateTrainTypeDto
    {
        public string Name { get; set; } = string.Empty;

        public int MaxSpeed { get; set; }

        public int Capacity { get; set; }

        public string Manufacturer { get; set; } = string.Empty;

        public TypeOfTrain Type { get; set; }
    }
}
