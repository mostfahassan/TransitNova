FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["UI/TransitNova.UI/TransitNova.UI.csproj", "UI/TransitNova.UI/"]
COPY ["UI/TransitNovaUI.BusinessLayer/TransitNovaUI.BusinessLayer.csproj", "UI/TransitNovaUI.BusinessLayer/"]
COPY ["Src/TransitNova.BusinessLayer/TransitNova.BusinessLayer.csproj", "Src/TransitNova.BusinessLayer/"]
COPY ["Src/TransitNova.Domain/TransitNova.Domain.csproj", "Src/TransitNova.Domain/"]

RUN dotnet restore "UI/TransitNova.UI/TransitNova.UI.csproj"

COPY ["UI/TransitNova.UI/", "UI/TransitNova.UI/"]
COPY ["UI/TransitNovaUI.BusinessLayer/", "UI/TransitNovaUI.BusinessLayer/"]
COPY ["Src/TransitNova.BusinessLayer/", "Src/TransitNova.BusinessLayer/"]
COPY ["Src/TransitNova.Domain/", "Src/TransitNova.Domain/"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UI/TransitNova.UI/TransitNova.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore --verbosity normal /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransitNova.UI.dll"]