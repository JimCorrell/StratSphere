using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;
using StratSphere.Data;
using StratSphere.Data.Repositories;
using StratSphere.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<StratSphereDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsAssembly("Stratsphere.Data")));

// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
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

// ── MVC ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
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
