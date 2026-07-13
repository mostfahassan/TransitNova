FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["Src/TransitNova.Api/TransitNova.Api.csproj", "TransitNova.Api/"]
COPY ["Src/TransitNova.BusinessLayer/TransitNova.BusinessLayer.csproj", "TransitNova.BusinessLayer/"]
COPY ["Src/TransitNova.Domain/TransitNova.Domain.csproj", "TransitNova.Domain/"]
COPY ["Src/TransitNova.InfraStructure/TransitNova.InfraStructure.csproj", "TransitNova.InfraStructure/"]

RUN dotnet restore "TransitNova.Api/TransitNova.Api.csproj"

COPY ["Src/TransitNova.Api/", "TransitNova.Api/"]
COPY ["Src/TransitNova.BusinessLayer/", "TransitNova.BusinessLayer/"]
COPY ["Src/TransitNova.Domain/", "TransitNova.Domain/"]
COPY ["Src/TransitNova.InfraStructure/", "TransitNova.InfraStructure/"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TransitNova.Api/TransitNova.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TransitNova.Api.dll"]

