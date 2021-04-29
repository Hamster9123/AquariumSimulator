FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine-amd64 AS build

COPY . /opt/aquarium
WORKDIR /opt/aquarium
RUN dotnet build --configuration Release

FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine-amd64
WORKDIR /opt/aquarium
COPY --from=build /opt/aquarium/bin/Release/net5.0 /opt/aquarium
COPY aquarium-simulator.json .

ENTRYPOINT ["./AquariumSimulator", "aquarium-simulator.json"]
