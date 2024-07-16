FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5174

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Api/BeaversTests.TestsManager.Api.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Api/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.App/BeaversTests.TestsManager.App.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.App/"]
COPY ["src/BuildingBlocks/BeaversTests.Common.CQRS/BeaversTests.Common.CQRS.csproj", "src/BuildingBlocks/BeaversTests.Common.CQRS/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Core/BeaversTests.TestsManager.Core.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Core/"]
COPY ["src/BuildingBlocks/BeaversTests.Common.Application/BeaversTests.Common.Application.csproj", "src/BuildingBlocks/BeaversTests.Common.Application/"]
COPY ["src/BuildingBlocks/BeaversTests.Extensions/BeaversTests.Extensions.csproj", "src/BuildingBlocks/BeaversTests.Extensions/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Infrastructure/BeaversTests.TestsManager.Infrastructure.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Infrastructure/"]
RUN dotnet restore "src/Services/TestsManager/BeaversTests.TestsManager.Api/BeaversTests.TestsManager.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/TestsManager/BeaversTests.TestsManager.Api"
RUN dotnet build "BeaversTests.TestsManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BeaversTests.TestsManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeaversTests.TestsManager.Api.dll"]
