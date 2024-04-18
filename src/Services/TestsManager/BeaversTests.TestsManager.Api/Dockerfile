FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Api/BeaversTests.TestsManager.Api.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Api/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.App/BeaversTests.TestsManager.App.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.App/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Core/BeaversTests.TestsManager.Core.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Core/"]
COPY ["src/Services/TestsManager/BeaversTests.TestsManager.Infrastructure/BeaversTests.TestsManager.Infrastructure.csproj", "src/Services/TestsManager/BeaversTests.TestsManager.Infrastructure/"]
RUN dotnet restore "src/Services/TestsManager/BeaversTests.TestsManager.Api/BeaversTests.TestsManager.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/TestsManager/BeaversTests.TestsManager.Api"
RUN dotnet build "BeaversTests.TestsManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeaversTests.TestsManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeaversTests.TestsManager.Api.dll"]
