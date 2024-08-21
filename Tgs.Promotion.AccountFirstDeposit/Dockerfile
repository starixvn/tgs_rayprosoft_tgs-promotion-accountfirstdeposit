FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.Promotion.AccountFirstDeposit/Tgs.Promotion.AccountFirstDeposit.csproj", "Tgs.Promotion.AccountFirstDeposit/"]
COPY ["AccountFirstDeposit.Core/AccountFirstDeposit.Core.csproj", "AccountFirstDeposit.Core/"]
COPY ["AccountFirstDeposit.Repositories/AccountFirstDeposit.Repositories.csproj", "AccountFirstDeposit.Repositories/"]
RUN dotnet restore "./Tgs.Promotion.AccountFirstDeposit/./Tgs.Promotion.AccountFirstDeposit.csproj"
COPY . .
WORKDIR "/src/Tgs.Promotion.AccountFirstDeposit"
RUN dotnet build "./Tgs.Promotion.AccountFirstDeposit.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.Promotion.AccountFirstDeposit.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Promotion.AccountFirstDeposit.dll"]