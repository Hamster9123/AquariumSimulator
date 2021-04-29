# Aquarium Simulator

Simple C# simulation of an aquarium with predators and herbivorous.

[Task description](https://github.com/Hamster9123/AquariumSimulator/blob/master/task.pdf)

## Requirements

- .Net 5.0 SDK
- dotnet cli tool

## Build

```bash
dotnet build --configuration Release
```

## Run

```bash
./bin/Release/net5.0/AquariumSimulator aquarium-simulator.json
```

## Run in Docker

```bash
$ sudo docker build . -t aquarium
$ sudo docker run -it aquarium aquarium-simulator.json
```