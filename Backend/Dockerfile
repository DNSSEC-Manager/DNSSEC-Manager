# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore
COPY DnssecManager.sln ./
COPY Backend/Backend.csproj Backend/
COPY Providers/Providers.csproj Providers/
COPY EppLib/EppLib.csproj EppLib/
RUN dotnet restore Backend/Backend.csproj

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish Backend/Backend.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create persistence folders (can also be mounted as volumes)
RUN mkdir -p /app/data && mkdir -p /app/Keys

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Default connection string can be overridden via env var or docker-compose
# ENV ConnectionStrings__DefaultConnection="Data Source=data/app.db"

ENTRYPOINT ["dotnet", "Backend.dll"]
