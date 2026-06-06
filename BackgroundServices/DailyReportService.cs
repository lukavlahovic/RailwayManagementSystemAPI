using System.Diagnostics;
using Microsoft.Extensions.Options;
using RailwayManagementSystemAPI.Configuration;
using RailwayManagementSystemAPI.Services;

namespace RailwayManagementSystemAPI.BackgroundServices
{
    public class DailyReportService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PythonSettings _pythonSettings;
        private readonly ILogger<DailyReportService> _logger;

        public DailyReportService(IServiceScopeFactory scopeFactory, IOptions<PythonSettings> pythonSettings, ILogger<DailyReportService> logger)
        {
            _scopeFactory = scopeFactory;
            _pythonSettings = pythonSettings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Daily Report Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextMidnight = now.AddDays(1);
                var delay = nextMidnight - now;

                _logger.LogInformation("Next report scheduled at {NextMidnight}", nextMidnight);

                await Task.Delay(delay, stoppingToken);

                await GenerateAndSendReportAsync();
            }
        }

        private async Task GenerateAndSendReportAsync()
        {
            var yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

            _logger.LogInformation("Generating daily report for {Date}", yesterday);

            var scriptDirectory = Path.GetDirectoryName(Path.GetFullPath(_pythonSettings.ScriptPath))!;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _pythonSettings.PythonPath,
                    Arguments = $"main.py --date {yesterday} --format pdf",
                    WorkingDirectory = scriptDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrEmpty(output))
                _logger.LogInformation("Python output: {Output}", output);

            if (!string.IsNullOrEmpty(error))
                _logger.LogWarning("Python error: {Error}", error);

            var pdfPath = process.ExitCode == 0
                ? Path.Combine(scriptDirectory, "output", $"report_{yesterday}.pdf")
                : null;

            using var scope = _scopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            await emailService.SendDailyReportAsync(yesterday, pdfPath);
        }
    }
}