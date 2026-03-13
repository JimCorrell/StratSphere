using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;
using StratSphere.Data;
using StratSphere.Data.Repositories;
using StratSphere.Web.Middleware;
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

// Apply any pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<StratSphereDbContext>().Database.MigrateAsync();

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
// Explicit league management routes must come before the slug route
app.MapControllerRoute("league-index",      "league",            new { controller = "League", action = "Index" });
app.MapControllerRoute("league-create",     "league/create",     new { controller = "League", action = "Create" });
app.MapControllerRoute("league-join",       "league/join",       new { controller = "League", action = "Join" });
app.MapControllerRoute("league-not-member", "league/not-member", new { controller = "League", action = "NotMember" });

// Detail routes with GUIDs need explicit routes so the GUID lands in {id}, not {action}
app.MapControllerRoute("team-detail",   "league/{slug}/team/{id:guid}",   new { controller = "Team",   action = "Detail" });
app.MapControllerRoute("season-detail", "league/{slug}/season/{id:guid}", new { controller = "Season", action = "Detail" });

app.MapControllerRoute(
    name: "league",
    pattern: "league/{slug}/{controller=League}/{action=Detail}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
