FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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

RUN dotnet publish "UI/TransitNova.UI/TransitNova.UI.csproj" -c Release -o /app/publish --no-restore --verbosity normal

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TransitNova.UI.dll"]
