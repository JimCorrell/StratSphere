# ── Build stage ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (layer-cached unless .csproj files change)
COPY ["src/StratSphere.Web/StratSphere.Web.csproj", "src/StratSphere.Web/"]
COPY ["src/StratSphere.Core/StratSphere.Core.csproj", "src/StratSphere.Core/"]
COPY ["src/StratSphere.Data/StratSphere.Data.csproj", "src/StratSphere.Data/"]
RUN dotnet restore "src/StratSphere.Web/StratSphere.Web.csproj"

# Copy everything else and publish
COPY . .
RUN dotnet publish "src/StratSphere.Web/StratSphere.Web.csproj" \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Run as non-root for security
RUN useradd -u 10001 -m -s /usr/sbin/nologin appuser
USER 10001

COPY --from=build --chown=10001:10001 /app/publish .
ENTRYPOINT ["dotnet", "StratSphere.Web.dll"]
