FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5175
USER root

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/TestRunnerController/BeaversTests.TestRunnerController.Api/BeaversTests.TestRunnerController.Api.csproj", "src/Services/TestRunnerController/BeaversTests.TestRunnerController.Api/"]
COPY ["src/Services/TestRunnerController/BeaversTests.TestRunnerController.App/BeaversTests.TestRunnerController.App.csproj", "src/Services/TestRunnerController/BeaversTests.TestRunnerController.App/"]
COPY ["src/Services/TestRunnerController/BeaversTests.TestRunnerController.Core/BeaversTests.TestRunnerController.Core.csproj", "src/Services/TestRunnerController/BeaversTests.TestRunnerController.Core/"]
COPY ["src/BuildingBlocks/BeaversTests.Common.CQRS/BeaversTests.Common.CQRS.csproj", "src/BuildingBlocks/BeaversTests.Common.CQRS/"]
COPY ["src/BuildingBlocks/EventStores/BeaversTests.Postgres.EventStore/BeaversTests.Postgres.EventStore.csproj", "src/BuildingBlocks/EventStores/BeaversTests.Postgres.EventStore/"]
COPY ["src/BuildingBlocks/BeaversTests.Common.CQRS.Abstractions/BeaversTests.Common.CQRS.Abstractions.csproj", "src/BuildingBlocks/BeaversTests.Common.CQRS.Abstractions/"]
COPY ["src/BuildingBlocks/BeaversTests.Common.Application/BeaversTests.Common.Application.csproj", "src/BuildingBlocks/BeaversTests.Common.Application/"]
COPY ["src/BuildingBlocks/MessageBrokers/BeaversTests.RabbitMQ.MessageBroker/BeaversTests.RabbitMQ.MessageBroker.csproj", "src/BuildingBlocks/MessageBrokers/BeaversTests.RabbitMQ.MessageBroker/"]
COPY ["src/Events/BeaversTests.TestsManager.Events/BeaversTests.TestsManager.Events.csproj", "src/Events/BeaversTests.TestsManager.Events/"]
COPY ["src/Events/BeaversTests.TestRunnerController.Events/BeaversTests.TestRunnerController.Events.csproj", "src/Events/BeaversTests.TestRunnerController.Events/"]
COPY ["src/Events/BeaversTests.TestRunnerAgent.Events/BeaversTests.TestRunnerAgent.Events.csproj", "src/Events/BeaversTests.TestRunnerAgent.Events/"]
COPY ["src/Services/TestRunnerController/BeaversTests.TestRunnerController.Infrastructure/BeaversTests.TestRunnerController.Infrastructure.csproj", "src/Services/TestRunnerController/BeaversTests.TestRunnerController.Infrastructure/"]
COPY ["src/BuildingBlocks/BeaversTests.Api.Shared/BeaversTests.Api.Shared.csproj", "src/BuildingBlocks/BeaversTests.Api.Shared/"]
RUN dotnet restore "src/Services/TestRunnerController/BeaversTests.TestRunnerController.Api/BeaversTests.TestRunnerController.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/TestRunnerController/BeaversTests.TestRunnerController.Api"
RUN dotnet build "BeaversTests.TestRunnerController.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BeaversTests.TestRunnerController.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeaversTests.TestRunnerController.Api.dll"]
