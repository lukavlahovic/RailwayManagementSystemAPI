using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RailwayManagementSystemAPI.Configuration;

namespace RailwayManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/report")]
    [Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly PythonSettings _pythonSettings;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IOptions<PythonSettings> pythonSettings, ILogger<ReportController> logger)
        {
            _pythonSettings = pythonSettings.Value;
            _logger = logger;
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport(
            [FromQuery] DateTime date,
            [FromQuery] string format = "pdf" )
        {
            if (format != "pdf" && format != "both")
            {
                return BadRequest("Format must be 'pdf' or 'both'");
            }

            var dateStr = date.ToString("yyyy-MM-dd");
            var outputFile = Path.Combine(_pythonSettings.ScriptPath
                .Replace("main.py", ""), 
                $"output/report_{dateStr}.{format}");

            _logger.LogInformation("Generating {Format} report for {Date}", format, dateStr);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _pythonSettings.PythonPath,
                    Arguments = $"main.py --date {dateStr} --format {format}",
                    WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(_pythonSettings.ScriptPath))!,
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

            if (process.ExitCode != 0)
            {
                _logger.LogWarning("Python script failed for date {Date}: {Error}", dateStr, error);
                return NotFound($"No completed trips found for {dateStr}");
            }

            _logger.LogInformation("Report generated successfully for {Date}", dateStr);

            if (!System.IO.File.Exists(outputFile))
                return NotFound($"Report file was not generated, name: {outputFile}");

            var contentType = format == "pdf" ? "application/pdf" : "text/html";
            var fileName = $"railway-report-{dateStr}.{format}";
            var fileBytes = await System.IO.File.ReadAllBytesAsync(outputFile);

            return File(fileBytes, contentType, fileName);
        }
    }
}