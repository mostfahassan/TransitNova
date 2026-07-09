
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/TransitNova.Api/TransitNova.Api.csproj", "Src/TransitNova.Api/"]
COPY ["Src/TransitNova.BusinessLayer/TransitNova.BusinessLayer.csproj", "Src/TransitNova.BusinessLayer/"]
COPY ["Src/TransitNova.Domain/TransitNova.Domain.csproj", "Src/TransitNova.Domain/"]
COPY ["Src/TransitNova.InfraStructure/TransitNova.InfraStructure.csproj", "Src/TransitNova.InfraStructure/"]
RUN dotnet restore "./Src/TransitNova.Api/TransitNova.Api.csproj"
COPY . .
WORKDIR "/src/Src/TransitNova.Api"
RUN dotnet build "./TransitNova.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TransitNova.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransitNova.Api.dll"]