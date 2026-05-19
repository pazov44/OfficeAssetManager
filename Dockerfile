# 1. Use the .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# CRITICAL: Enable preview features so the dotnet CLI can read .slnx files
ENV DOTNET_ENABLE_PREVIEW_FEATURES=1

# 2. Copy the solution file and restore dependencies
COPY *.slnx ./
COPY src/backend/OfficeAssetManager.Api/*.csproj ./src/backend/OfficeAssetManager.Api/
COPY src/backend/OfficeAssetManager.Core/*.csproj ./src/backend/OfficeAssetManager.Core/
COPY src/backend/OfficeAssetManager.Infrastructure/*.csproj ./src/backend/OfficeAssetManager.Infrastructure/

# Run restore pointing directly to the Api project if slnx parsing fails
RUN dotnet restore src/backend/OfficeAssetManager.Api/OfficeAssetManager.Api.csproj

# 3. Copy everything else and build the release
COPY . ./
WORKDIR /app/src/backend/OfficeAssetManager.Api
RUN dotnet publish OfficeAssetManager.Api.csproj -c Release -o /app/out

# 4. Use the runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "OfficeAssetManager.Api.dll"]