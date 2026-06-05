namespace RailwayManagementSystemAPI.Dtos
{
    public class ReportQuery
    {
        public DateTime DateTime { get; set; } = DateTime.Today;
        public string Format { get; set; } = "pdf";
    }
}