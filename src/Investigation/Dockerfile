FROM mcr.microsoft.com/dotnet/runtime:9.0

WORKDIR /

RUN mkdir -p /TestTests /TestTestsRunner

COPY ./TestTests/bin/Debug/net9.0/ /TestTests/

COPY ./TestTestsRunner/bin/Debug/net9.0/ /TestTestsRunner/

WORKDIR /TestTestsRunner
ENTRYPOINT ["dotnet", "TestTestsRunner.dll", "/TestTests/TestTests.dll"]