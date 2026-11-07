# syntax=docker/dockerfile:1

# =============================================================================
# Personal Portfolio — optimized multi-stage build (.NET 10)
#
# Stages:
#   build    : restore + compile the full solution (Release)
#   test     : run the NUnit unit-test suite (blocks image build on failure)
#   publish  : emit trimmed self-contained outputs for Portfolio.Web
#   final    : minimal ASP.NET Core runtime image, runs as non-root
# =============================================================================

# ---------- Stage 1: build --------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first so the restore layer is cached independently of source changes.
COPY ["PersonalPortfolio.sln", "./"]
COPY ["src/Portfolio.Domain/Portfolio.Domain.csproj", "src/Portfolio.Domain/"]
COPY ["src/Portfolio.Application/Portfolio.Application.csproj", "src/Portfolio.Application/"]
COPY ["src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj", "src/Portfolio.Infrastructure/"]
COPY ["src/Portfolio.Web/Portfolio.Web.csproj", "src/Portfolio.Web/"]
COPY ["tests/Portfolio.UnitTests/Portfolio.UnitTests.csproj", "tests/Portfolio.UnitTests/"]
RUN dotnet restore PersonalPortfolio.sln --nologo

# Copy the remaining source and compile everything.
COPY . .
RUN dotnet build PersonalPortfolio.sln -c Release --no-restore --nologo

# ---------- Stage 2: test ---------------------------------------------------
# Unit tests run as a distinct stage so a failing test aborts the image build.
FROM build AS test
WORKDIR /src/tests/Portfolio.UnitTests
RUN dotnet test --no-build -c Release --nologo --logger "console;verbosity=normal"

# ---------- Stage 3: publish ------------------------------------------------
FROM build AS publish
WORKDIR /src
RUN dotnet publish src/Portfolio.Web/Portfolio.Web.csproj -c Release --no-restore -o /app/publish

# ---------- Stage 4: final runtime image ------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

# Serilog writes its rolling file sink under /app/logs — make it writable by the
# non-root app user that ships with the .NET images (uid $APP_UID).
RUN mkdir -p /app/logs && chown -R $APP_UID:$APP_UID /app
USER $APP_UID

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# DbInitializer inside the app applies EF Core migrations and seeds the admin
# account on startup — no entrypoint scripts needed.
ENTRYPOINT ["dotnet", "Portfolio.Web.dll"]
