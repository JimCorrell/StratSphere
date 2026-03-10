# ── Build stage ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (layer-cached unless .csproj files change)
COPY ["src/Stratsphere.Web/Stratsphere.Web.csproj",   "src/Stratsphere.Web/"]
COPY ["src/StratSphere.Core/StratSphere.Core.csproj", "src/StratSphere.Core/"]
COPY ["src/Stratsphere.Data/Stratsphere.Data.csproj", "src/Stratsphere.Data/"]
RUN dotnet restore "src/Stratsphere.Web/Stratsphere.Web.csproj"

# Copy everything else and publish
COPY . .
RUN dotnet publish "src/Stratsphere.Web/Stratsphere.Web.csproj" \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Stratsphere.Web.dll"]
