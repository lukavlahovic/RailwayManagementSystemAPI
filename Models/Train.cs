using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.Models
{
    public class Train
    {
        public int Id { get; set; }

        [Required]
        public int TrainTypeId { get; set; }

        public TrainType TrainType { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string SerialNumber { get; set; } = string.Empty;
    }
}
