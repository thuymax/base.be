# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csproj files first (layer caching)
COPY ["SEEDONE.API/SEEDONE.API.csproj", "SEEDONE.API/"]
COPY ["SEEDONE.SERVICE/SEEDONE.Web.Service.csproj", "SEEDONE.SERVICE/"]
COPY ["SEEDONE.REPO/SEEDONE.Web.Repo.csproj", "SEEDONE.REPO/"]
RUN dotnet restore "SEEDONE.API/SEEDONE.API.csproj"

# Copy all source and publish
COPY . .
RUN dotnet publish "SEEDONE.API/SEEDONE.API.csproj" -c Release -o /app/publish --no-restore

# Runtime stage — slim alpine image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
EXPOSE 80

# Create non-root user for security
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
RUN mkdir -p /app/uploads && chown appuser:appgroup /app/uploads

COPY --from=build /app/publish .

USER appuser
ENTRYPOINT ["dotnet", "SEEDONE.API.dll"]
