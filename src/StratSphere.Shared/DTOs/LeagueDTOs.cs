using StratSphere.Shared.Enums;

namespace StratSphere.Shared.DTOs;

public enum LeagueRole
{
    Commissioner,
    CoCommissioner,
    Member
}

// League DTOs
public record CreateLeagueRequest(
    string Name,
    string? Description,
    int MaxTeams = 30,
    int RosterSize = 40,
    int ActiveRosterSize = 25,
    bool UseDH = true
);

public record UpdateLeagueRequest(
    string? Name,
    string? Description,
    int? MaxTeams,
    int? RosterSize,
    int? ActiveRosterSize,
    bool? UseDH,
    LeagueStatus? Status
);

public record LeagueResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    LeagueStatus Status,
    int CurrentSeason,
    SeasonPhase CurrentPhase,
    int MaxTeams,
    int RosterSize,
    int ActiveRosterSize,
    bool UseDH,
    int TeamCount,
    int MemberCount,
    DateTime CreatedAt
);

public record LeagueSummaryResponse(
    Guid Id,
    string Name,
    string Slug,
    LeagueStatus Status,
    int TeamCount
);

// Subleague DTOs
public record CreateSubleagueRequest(
    string Name,
    string? Abbreviation
);

public record UpdateSubleagueRequest(
    string? Name,
    string? Abbreviation
);

public record SubleagueResponse(
    Guid Id,
    string Name,
    string? Abbreviation,
    int DivisionCount
);

// Division DTOs
public record CreateDivisionRequest(
    string Name,
    string? Abbreviation
);

public record UpdateDivisionRequest(
    string? Name,
    string? Abbreviation
);

public record DivisionResponse(
    Guid Id,
    string Name,
    string? Abbreviation,
    int TeamCount
);

// Team DTOs
public record CreateTeamRequest(
    string Name,
    string Abbreviation,
    string? City,
    string? Conference,
    Guid? SubleagueId,
    Guid? DivisionId,
    string? DivisionName
);

public record UpdateTeamRequest(
    string? Name,
    string? Abbreviation,
    string? City,
    string? Conference,
    Guid? SubleagueId,
    Guid? DivisionId,
    string? DivisionName,
    string? LogoUrl
);

public record TeamResponse(
    Guid Id,
    string Name,
    string Abbreviation,
    string? City,
    string? LogoUrl,
    Guid? SubleagueId,
    string? SubleagueName,
    Guid? DivisionId,
    string? DivisionName,
    string? Conference,
    Guid OwnerId,
    string OwnerName,
    int RosterCount
);

// User DTOs
public record RegisterUserRequest(
    string Email,
    string Username,
    string Password,
    string DisplayName
);

public record LoginRequest(
    string EmailOrUsername,
    string Password
);

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    UserResponse User
);

public record UserResponse(
    Guid Id,
    string Email,
    string Username,
    string DisplayName,
    DateTime CreatedAt,
    Guid? LastVisitedLeagueId = null
);

// League Member DTOs
public record AddMemberRequest(
    Guid UserId,
    LeagueRole Role = LeagueRole.Member
);

public record LeagueMemberResponse(
    Guid Id,
    Guid UserId,
    string Username,
    string DisplayName,
    LeagueRole Role,
    DateTime JoinedAt,
    bool IsActive
);
