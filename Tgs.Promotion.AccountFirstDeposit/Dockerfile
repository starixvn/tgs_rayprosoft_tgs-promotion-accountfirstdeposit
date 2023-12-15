#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Promotion.AccountFirstDeposit.dll"]