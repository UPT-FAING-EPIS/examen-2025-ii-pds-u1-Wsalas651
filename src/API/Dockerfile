FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["API.csproj", "API/"]
COPY ["..\/Core/Core.csproj", "Core/"]
COPY ["..\/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "API/API.csproj"

COPY . ./API
COPY ../Core/ ./Core/
COPY ../Infrastructure/ ./Infrastructure/

WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Crear un archivo de configuración vacío que será reemplazado por variables de entorno
RUN echo '{"ConnectionStrings":{"DefaultConnection":""},"Jwt":{"Key":"","Issuer":"","Audience":"","ExpiryMinutes":0},"Logging":{"LogLevel":{"Default":"Information","Microsoft":"Warning","Microsoft.Hosting.Lifetime":"Information"}},"AllowedHosts":"*"}' > appsettings.json

ENTRYPOINT ["dotnet", "API.dll"]