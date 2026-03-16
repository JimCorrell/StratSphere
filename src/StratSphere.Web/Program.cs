using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;
using StratSphere.Data;
using StratSphere.Data.Repositories;
using StratSphere.Web.Middleware;
using StratSphere.Web.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<StratSphereDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsAssembly("StratSphere.Data")));

// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequiredLength = 12;
    options.Password.RequiredUniqueChars = 4;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<StratSphereDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/access-denied";
});

// ── Repositories ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<ILeagueRepository,     LeagueRepository>();
builder.Services.AddScoped<ISeasonRepository,     SeasonRepository>();
builder.Services.AddScoped<ITeamRepository,        TeamRepository>();
builder.Services.AddScoped<IStandingsRepository,   StandingsRepository>();
builder.Services.AddScoped<IRosterRepository,      RosterRepository>();
builder.Services.AddScoped<IPlayerCardRepository,  PlayerCardRepository>();
builder.Services.AddScoped<ILahmanRepository,      LahmanRepository>();

// ── Email ─────────────────────────────────────────────────────────────────────
builder.Services.AddTransient<IEmailSender<ApplicationUser>, LoggingEmailSender>();

// ── Seed ──────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<DataSeeder>();

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<LeagueService>();
builder.Services.AddScoped<StandingsService>();
builder.Services.AddScoped<RosterService>();
builder.Services.AddScoped<PlayerCardService>();

// ── Rate limiting ─────────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    // 10 login attempts per IP per 15-minute window
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(15),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ── MVC ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply any pending EF Core migrations, then seed required data
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<StratSphereDbContext>().Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<DataSeeder>().SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Inject active league into HttpContext for league-scoped routes
app.UseMiddleware<LeagueContextMiddleware>();

// ── Routes ────────────────────────────────────────────────────────────────────
// ── League (non-scoped) ───────────────────────────────────────────────────────
app.MapControllerRoute("league-index",      "league",            new { controller = "League", action = "Index" });
app.MapControllerRoute("league-create",     "league/create",     new { controller = "League", action = "Create" });
app.MapControllerRoute("league-join",       "league/join",       new { controller = "League", action = "Join" });
app.MapControllerRoute("league-not-member", "league/not-member", new { controller = "League", action = "NotMember" });
app.MapControllerRoute("league-delete-all", "league/delete-all", new { controller = "League", action = "DeleteAllLeagues" });

// ── League (scoped by abbreviation) ──────────────────────────────────────────
app.MapControllerRoute("league-archive",             "league/{leagueAbbr}/archive",             new { controller = "League", action = "Archive" });
app.MapControllerRoute("league-unarchive",           "league/{leagueAbbr}/unarchive",           new { controller = "League", action = "Unarchive" });
app.MapControllerRoute("league-delete",              "league/{leagueAbbr}/delete",              new { controller = "League", action = "DeleteLeague" });
app.MapControllerRoute("league-assign-commissioner", "league/{leagueAbbr}/assign-commissioner", new { controller = "League", action = "AssignCommissioner" });

// ── Season ("create" before {year:int} so the literal doesn't match as int) ──
app.MapControllerRoute("season-create",   "league/{leagueAbbr}/season/create",              new { controller = "Season", action = "Create" });
app.MapControllerRoute("season-activate", "league/{leagueAbbr}/season/{year:int}/activate", new { controller = "Season", action = "Activate" });
app.MapControllerRoute("season-complete", "league/{leagueAbbr}/season/{year:int}/complete", new { controller = "Season", action = "Complete" });
app.MapControllerRoute("season-detail",   "league/{leagueAbbr}/season/{year:int}",          new { controller = "Season", action = "Detail" });

// ── Team ──────────────────────────────────────────────────────────────────────
app.MapControllerRoute("team-create", "league/{leagueAbbr}/team/create",                             new { controller = "Team", action = "Create" });
app.MapControllerRoute("team-claim",  "league/{leagueAbbr}/season/{year:int}/team/{teamAbbr}/claim", new { controller = "Team", action = "Claim" });
app.MapControllerRoute("team-detail", "league/{leagueAbbr}/season/{year:int}/team/{teamAbbr}",       new { controller = "Team", action = "Detail" });

// ── Roster ────────────────────────────────────────────────────────────────────
app.MapControllerRoute("roster-search", "league/{leagueAbbr}/Roster/SearchCards", new { controller = "Roster", action = "SearchCards" });
app.MapControllerRoute("roster-add",    "league/{leagueAbbr}/Roster/AddPlayer",   new { controller = "Roster", action = "AddPlayer" });
app.MapControllerRoute("roster-drop",   "league/{leagueAbbr}/Roster/Drop",        new { controller = "Roster", action = "Drop" });

// ── League detail (must be last league route) ─────────────────────────────────
app.MapControllerRoute("league-detail", "league/{leagueAbbr}", new { controller = "League", action = "Detail" });

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
