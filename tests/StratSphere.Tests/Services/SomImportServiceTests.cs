using StratSphere.Web.Services;

namespace StratSphere.Tests.Services;

public class SomImportServiceTests
{
    [Fact]
    public void Parse_RepositorySampleArchive_ReadsGamesAndPlayerStatistics()
    {
        var repositoryRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var archivePath = Path.Combine(repositoryRoot, "som", "2026US-5-5-2026.lzp");

        Assert.True(File.Exists(archivePath), $"Sample SOM archive not found at {archivePath}.");

        using var archiveStream = File.OpenRead(archivePath);
        var result = new SomImportService().Parse(archiveStream);

        Assert.NotEmpty(result.TeamOrder);
        Assert.NotEmpty(result.Games);
        Assert.NotEmpty(result.Batting);
        Assert.NotEmpty(result.Pitching);
    }
}
