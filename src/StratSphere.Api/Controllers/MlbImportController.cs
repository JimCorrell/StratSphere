using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratSphere.Infrastructure.Services;

namespace StratSphere.Api.Controllers;

/// <summary>
/// Manages Lahman historical baseball database imports
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MlbImportController : ControllerBase
{
    private readonly IMlbImportService _importService;
    private readonly ILogger<MlbImportController> _logger;

    public MlbImportController(IMlbImportService importService, ILogger<MlbImportController> logger)
    {
        _importService = importService;
        _logger = logger;
    }

    /// <summary>
    /// Import all Lahman CSV files from the specified directory
    /// </summary>
    /// <remarks>
    /// This endpoint imports all Lahman baseball data (People, Teams, Batting, Pitching, Fielding, Allstars, Hall of Fame)
    /// into the StratSphere database. Call this once to populate the MLB Historical Dataset.
    ///
    /// Example directory path: /Users/jimcorrell/Development/neho/StratSphere/lahmanDB/lahman_1871-2024u_csv
    /// </remarks>
    [HttpPost("import")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ImportAllMlbData([FromBody] MlbImportRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CsvDirectoryPath))
        {
            return BadRequest(new { error = "csvDirectoryPath is required" });
        }

        try
        {
            _logger.LogInformation("Starting MLB data import from {DirectoryPath}", request.CsvDirectoryPath);
            var result = await _importService.ImportMlbDataAsync(request.CsvDirectoryPath, cancellationToken);

            if (!result.Success)
            {
                _logger.LogWarning("MLB import completed with errors: {Errors}", string.Join("; ", result.Errors));
            }
            else
            {
                _logger.LogInformation("MLB import completed successfully in {Duration}ms", result.Duration.TotalMilliseconds);
            }

            return Ok(new
            {
                success = result.Success,
                duration = result.Duration,
                tablesProcessed = result.TotalTablesProcessed,
                rowsImported = result.RowsImportedByTable,
                errors = result.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Lahman data");
            return StatusCode(500, new { error = "Failed to import MLB data", details = ex.Message });
        }
    }
}

public class MlbImportRequest
{
    /// <summary>
    /// Full path to the directory containing Lahman CSV files
    /// </summary>
    public string? CsvDirectoryPath { get; set; }
}
