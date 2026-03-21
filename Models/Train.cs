namespace RailwayManagementSystemAPI.Models
{
    public enum TrainType
    {
        Passenger,
        Freight,
        HighSpeed,
        Commuter
    }

    public class Train
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int MaxSpeed { get; set; }
        public int Capacity { get; set; }
        public TrainType Type { get; set; }
    }
}
