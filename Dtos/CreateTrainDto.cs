using RailwayManagementSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Dtos
{
    public class CreateTrainDto
    {
        public int TrainTypeId { get; set; }

        public string SerialNumber { get; set; } = string.Empty;
    }
}
