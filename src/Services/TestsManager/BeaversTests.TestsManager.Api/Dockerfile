FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5174
USER root

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Api/BeaversTests.TestsManager.Api.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Api/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Core/BeaversTests.TestsManager.Core.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Core/"]
COPY ["src/BuildingBlocks/BeaversTests.Common.CQRS/BeaversTests.Common.CQRS.csproj", "src/BuildingBlocks/BeaversTests.Common.CQRS/"]
COPY ["src/BuildingBlocks/EventStores/BeaversTests.Postgres.EventStore/BeaversTests.Postgres.EventStore.csproj", "src/BuildingBlocks/EventStores/BeaversTests.Postgres.EventStore/"]
COPY ["src/BuildingBlocks/BeaversTests.Common.Application/BeaversTests.Common.Application.csproj", "src/BuildingBlocks/BeaversTests.Common.Application/"]
COPY ["src/BuildingBlocks/BeaversTests.Common.CQRS.Abstractions/BeaversTests.Common.CQRS.Abstractions.csproj", "src/BuildingBlocks/BeaversTests.Common.CQRS.Abstractions/"]
COPY ["src/BuildingBlocks/MessageBrokers/BeaversTests.RabbitMQ.MessageBroker/BeaversTests.RabbitMQ.MessageBroker.csproj", "src/BuildingBlocks/MessageBrokers/BeaversTests.RabbitMQ.MessageBroker/"]
COPY ["src/Events/BeaversTests.TestsManager.Events/BeaversTests.TestsManager.Events.csproj", "src/Events/BeaversTests.TestsManager.Events/"]
COPY ["src/BuildingBlocks/BeaversTests.TestDrivers.Internal/BeaversTests.TestDrivers.Internal.csproj", "src/BuildingBlocks/BeaversTests.TestDrivers.Internal/"]
COPY ["src/Public/BeaversTests.TestDrivers/BeaversTests.TestDrivers.csproj", "src/Public/BeaversTests.TestDrivers/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.App/BeaversTests.TestsManager.App.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.App/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Infrastructure/BeaversTests.TestsManager.Infrastructure.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Infrastructure/"]

#Drivers
COPY ["src/BuildingBlocks/Drivers/BeaversTests.NUnit.Driver/BeaversTests.NUnit.Driver.csproj", "src/BuildingBlocks/Drivers/BeaversTests.NUnit.Driver/"]

RUN dotnet restore "src/Services/TestsManager/BeaversTests.TestsManager.Api/BeaversTests.TestsManager.Api.csproj"
RUN dotnet restore "src/BuildingBlocks/Drivers/BeaversTests.NUnit.Driver/BeaversTests.NUnit.Driver.csproj"
COPY . .
WORKDIR "/src/src/BuildingBlocks/Drivers/BeaversTests.NUnit.Driver"
RUN dotnet build "BeaversTests.NUnit.Driver.csproj" -c $BUILD_CONFIGURATION -o /app/build/drivers
WORKDIR "/src/src/Services/TestsManager/BeaversTests.TestsManager.Api"
RUN dotnet build "BeaversTests.TestsManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/BuildingBlocks/Drivers/BeaversTests.NUnit.Driver"
RUN dotnet publish "BeaversTests.NUnit.Driver.csproj" -c $BUILD_CONFIGURATION -o /app/publish/drivers
WORKDIR "/src/src/Services/TestsManager/BeaversTests.TestsManager.Api"
RUN dotnet publish "BeaversTests.TestsManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeaversTests.TestsManager.Api.dll"]
