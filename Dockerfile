FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NuGet.Config ./
COPY backend/Sace.Api.csproj backend/
RUN dotnet restore backend/Sace.Api.csproj --configfile NuGet.Config

COPY backend/ backend/
RUN dotnet publish backend/Sace.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    && rm -f /app/publish/appsettings.Development.json \
        /app/publish/*.pdb \
        /app/publish/web.config \
        /app/publish/*.staticwebassets.*

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/data/storage /app/data/keys \
    && chown -R app:app /app

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000
USER app
ENTRYPOINT ["dotnet", "Sace.Api.dll"]
