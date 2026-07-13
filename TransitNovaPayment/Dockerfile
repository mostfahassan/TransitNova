FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["TransitNovaPayment/TransitNovaPayment.API/TransitNovaPayment.API.csproj", "TransitNovaPayment.API/"]
COPY ["TransitNovaPayment/TransitNovaPayment.Busieness/TransitNovaPayment.Busieness.csproj", "TransitNovaPayment.Busieness/"]
COPY ["TransitNovaPayment/TransitNovaPayment.Infrastructure/TransitNovaPayment.Infrastructure.csproj", "TransitNovaPayment.Infrastructure/"]

RUN dotnet restore "TransitNovaPayment.API/TransitNovaPayment.API.csproj"

COPY ["TransitNovaPayment/TransitNovaPayment.API/", "TransitNovaPayment.API/"]
COPY ["TransitNovaPayment/TransitNovaPayment.Busieness/", "TransitNovaPayment.Busieness/"]
COPY ["TransitNovaPayment/TransitNovaPayment.Infrastructure/", "TransitNovaPayment.Infrastructure/"]


RUN dotnet publish "TransitNovaPayment.API/TransitNovaPayment.API.csproj" -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "TransitNovaPayment.API.dll"]

