FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Src/TransitNova.Api/TransitNova.Api.csproj", "TransitNova.Api/"]
COPY ["Src/TransitNova.BusinessLayer/TransitNova.BusinessLayer.csproj", "TransitNova.BusinessLayer/"]
COPY ["Src/TransitNova.Domain/TransitNova.Domain.csproj", "TransitNova.Domain/"]
COPY ["Src/TransitNova.InfraStructure/TransitNova.InfraStructure.csproj", "TransitNova.InfraStructure/"]

RUN dotnet restore "TransitNova.Api/TransitNova.Api.csproj"

COPY ["Src/TransitNova.Api/", "TransitNova.Api/"]
COPY ["Src/TransitNova.BusinessLayer/", "TransitNova.BusinessLayer/"]
COPY ["Src/TransitNova.Domain/", "TransitNova.Domain/"]
COPY ["Src/TransitNova.InfraStructure/", "TransitNova.InfraStructure/"]


RUN dotnet publish "TransitNova.Api/TransitNova.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TransitNova.Api.dll"]