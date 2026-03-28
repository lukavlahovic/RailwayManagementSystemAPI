namespace RailwayManagementSystemAPI.Dtos
{
    public class TrainResponseDto
    {
        public int Id { get; set; }

        public string SerialNumber { get; set; } = string.Empty;

        public TrainTypeResponseDto TrainType { get; set; } = null!;
    }
}
