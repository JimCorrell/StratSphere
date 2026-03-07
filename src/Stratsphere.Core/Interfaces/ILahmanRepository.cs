using StratSphere.Core.Entities.Lahman;

namespace StratSphere.Core.Interfaces;

public interface ILahmanRepository
{
    Task<LahmanPerson?> GetPersonAsync(string playerId);
    Task<IEnumerable<LahmanPerson>> SearchPeopleAsync(string query, int limit = 20);
    Task<IEnumerable<LahmanBatting>> GetBattingAsync(string playerId);
    Task<LahmanBatting?> GetBattingSeasonAsync(string playerId, int yearId);
    Task<IEnumerable<LahmanPitching>> GetPitchingAsync(string playerId);
    Task<LahmanPitching?> GetPitchingSeasonAsync(string playerId, int yearId);
    Task<IEnumerable<LahmanFielding>> GetFieldingSeasonAsync(string playerId, int yearId);

    /// <summary>
    /// Search for players who have batting or pitching stats in a given year.
    /// Used for the card selection typeahead: "find me batters from 1986 named 'schm'".
    /// </summary>
    Task<IEnumerable<(LahmanPerson Person, string PrimaryPosition)>> SearchCardsAsync(
        string nameQuery, int? cardYear, bool pitchersOnly = false, int limit = 20);
}
