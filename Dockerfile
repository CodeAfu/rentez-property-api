FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["RentEZApi.csproj", "./"]
RUN dotnet restore "RentEZApi.csproj"
COPY . .
RUN dotnet publish "RentEZApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Security: run as non-root
USER $APP_UID

COPY --from=build /app/publish .
# RUN ls -la && cat appsettings.json || echo "appsettings.json not found"

ENTRYPOINT ["dotnet", "RentEZApi.dll"]