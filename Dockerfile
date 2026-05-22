# ─────────────────────────────────────────────────────────────────────────────
# Stage 1 — Build the React frontend
# ─────────────────────────────────────────────────────────────────────────────
FROM node:20-alpine AS frontend-build

WORKDIR /app/client

# Install dependencies first (better layer caching)
COPY ScatterbrainMusic.Client/package*.json ./
RUN npm ci

# Copy source and build
COPY ScatterbrainMusic.Client/ ./
RUN npm run build
# Output is in /app/client/dist


# ─────────────────────────────────────────────────────────────────────────────
# Stage 2 — Build & publish the .NET 8 API
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS api-build

WORKDIR /app

# Restore dependencies (separate layer for caching)
COPY ScatterbrainMusic.sln ./
COPY ScatterbrainMusic.API/ScatterbrainMusic.API.csproj           ScatterbrainMusic.API/
COPY ScatterbrainMusic.Tests/ScatterbrainMusic.Tests.csproj       ScatterbrainMusic.Tests/
RUN dotnet restore

# Copy all source and publish
COPY ScatterbrainMusic.API/   ScatterbrainMusic.API/
COPY ScatterbrainMusic.Tests/ ScatterbrainMusic.Tests/

# Run tests before publishing (fail the build if tests fail)
RUN dotnet test ScatterbrainMusic.Tests/ScatterbrainMusic.Tests.csproj \
      --no-restore \
      --configuration Release \
      --logger "console;verbosity=normal"

RUN dotnet publish ScatterbrainMusic.API/ScatterbrainMusic.API.csproj \
      --no-restore \
      --configuration Release \
      --output /app/publish


# ─────────────────────────────────────────────────────────────────────────────
# Stage 3 — Final runtime image
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

# Add non-root user for security
RUN addgroup -S scatterbrain && adduser -S scatterbrain -G scatterbrain

WORKDIR /app

# Copy the published API
COPY --from=api-build /app/publish ./

# Copy the built React app into wwwroot so Kestrel serves it as static files
COPY --from=frontend-build /app/client/dist ./wwwroot

# Set ownership
RUN chown -R scatterbrain:scatterbrain /app
USER scatterbrain

# Kestrel listens on HTTP 8080 (non-privileged port)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "ScatterbrainMusic.API.dll"]
