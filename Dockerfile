FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy fake build and restore
COPY build.fsx .
COPY .config/ .config/

RUN dotnet tool restore

# copy source stuff
COPY src/ src/
RUN dotnet fake build

ENTRYPOINT ["dotnet", "fake", "build", "-t", "CheckCodeFormat"]
