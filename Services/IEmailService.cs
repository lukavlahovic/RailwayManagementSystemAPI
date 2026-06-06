namespace RailwayManagementSystemAPI.Services
{
    public interface IEmailService
    {
        Task SendDailyReportAsync(string date, string? attachmentPath);
    }
}