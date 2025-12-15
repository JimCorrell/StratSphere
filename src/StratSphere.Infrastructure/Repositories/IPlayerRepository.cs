using StratSphere.Core.Entities;

namespace StratSphere.Infrastructure.Repositories;

public interface IPlayerRepository
{
    Task<List<MlbPeople>> GetHallOfFameAsync();
    Task<List<MlbBatting>> GetPlayerBattingStatsAsync(string playerId, int? startYear = null, int? endYear = null);
    Task<MlbPeople?> GetPlayerByIdAsync(int id);
    Task<List<MlbFielding>> GetPlayerFieldingStatsAsync(string playerId, int? startYear = null, int? endYear = null);
    Task<List<MlbPitching>> GetPlayerPitchingStatsAsync(string playerId, int? startYear = null, int? endYear = null);
    Task<List<MlbPeople>> GetPlayersByPositionAsync(string position, int limit = 50);
    Task SaveChangesAsync();
    Task<List<MlbPeople>> SearchPlayersAsync(string searchTerm, int limit = 50);
}
