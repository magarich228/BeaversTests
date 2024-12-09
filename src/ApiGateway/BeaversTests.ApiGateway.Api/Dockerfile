FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5008

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ApiGateway/BeaversTests.ApiGateway.Api/BeaversTests.ApiGateway.Api.csproj", "src/ApiGateway/BeaversTests.ApiGateway.Api/"]
RUN dotnet restore "src/ApiGateway/BeaversTests.ApiGateway.Api/BeaversTests.ApiGateway.Api.csproj"
COPY . .
WORKDIR "/src/src/ApiGateway/BeaversTests.ApiGateway.Api"
RUN dotnet build "BeaversTests.ApiGateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BeaversTests.ApiGateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeaversTests.ApiGateway.Api.dll"]
