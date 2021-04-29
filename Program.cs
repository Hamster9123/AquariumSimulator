using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using AquariumSimulator.Model;
using AquariumSimulator.View;


namespace AquariumSimulator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const int timeout = 1000;

            // Check arguments
            if (args.Length < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Config file path wasn't set!");
                Console.ResetColor();
                Console.WriteLine("\nProgram usage example:\n" +
                                  "\tLinux: ./AquariumSimulator aquarium-simulator.json\n" +
                                  "\tWindows: AquariumSimulator.exe aquarium-simulator.json");
            }

            // Get config full path
            string configPath = Path.GetFullPath(args[0]);
            if (!File.Exists(configPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"File {configPath} doesn't exists!");
                Console.ResetColor();
            }

            try
            {
                // Parse config file
                JObject config = JObject.Parse(File.ReadAllText(configPath));
                uint width = (uint) config["Width"];
                uint height = (uint) config["Height"];
                List<AquaObject> objects = ParseAquaObjects(config);

                if (objects is null || objects.Count == 0)
                {
                    Console.WriteLine("No objects are in config, stop processing.");
                    return;
                }

                // Initialize an aquarium and a view
                Aquarium aquarium = new Aquarium(width, height);
                IView view = new CliView();

                // Fill aquarium with objects
                foreach (var obj in objects)
                {
                    aquarium.AddObject(obj);
                }

                // Start main loop with drawing
                bool stopKeyPressed = false;
                List<char> stopKeys = new List<char> {'q', 'Q'};
                while (!stopKeyPressed)
                {
                    view.DrawAquarium(aquarium);

                    Thread.Sleep(timeout);
                    if (Console.KeyAvailable && stopKeys.Contains(Console.ReadKey().KeyChar))
                    {
                        stopKeyPressed = true;
                    }

                    aquarium.NextStage();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private static List<AquaObject> ParseAquaObjects(JObject config)
        {
            List<AquaObject> objects = new List<AquaObject>();
            JArray objectDefaults = (JArray) config["ObjectDefaults"] ?? new JArray();
            JArray aquaObjects = (JArray) config["Objects"] ?? new JArray();
            var defaultsDict = new Dictionary<string, AquaObjectConfig>();

            // Parse defaults
            foreach (JToken defaults in objectDefaults)
            {
                switch (defaults["Type"]?.ToString())
                {
                    case "Seaweed":
                        defaultsDict["Seaweed"] = SeaweedConfig.Fill(defaults, new SeaweedConfig());
                        break;
                    case "HerbivorousFish":
                        defaultsDict["HerbivorousFish"] = FishConfig.Fill(defaults, new FishConfig());
                        break;
                    case "PredatoryFish":
                        defaultsDict["PredatoryFish"] = FishConfig.Fill(defaults, new FishConfig());
                        break;
                    default:
                        throw new ArgumentException("Defaults object has no type");
                }
            }

            // Parse objects
            foreach (JToken objJson in aquaObjects)
            {
                AquaObjectConfig defaultConfig;
                AquaObjectConfig objectConfig;
                AquaObject obj;

                switch (objJson["Type"]?.ToString())
                {
                    case "Seaweed":
                        defaultConfig = defaultsDict.ContainsKey("Seaweed")
                            ? defaultsDict["Seaweed"]
                            : new SeaweedConfig();
                        objectConfig = SeaweedConfig.Fill(objJson, (SeaweedConfig) defaultConfig);
                        obj = new Seaweed((SeaweedConfig) objectConfig);
                        break;
                    case "Terrain":
                        defaultConfig = defaultsDict.ContainsKey("Terrain")
                            ? defaultsDict["Terrain"]
                            : new TerrainConfig();
                        objectConfig = TerrainConfig.Fill(objJson, (TerrainConfig) defaultConfig);
                        obj = new Terrain((TerrainConfig) objectConfig);
                        break;
                    case "HerbivorousFish":
                        defaultConfig = defaultsDict.ContainsKey("HerbivorousFish")
                            ? defaultsDict["HerbivorousFish"]
                            : new FishConfig();
                        objectConfig = FishConfig.Fill(objJson, (FishConfig) defaultConfig);
                        obj = new HerbivorousFish((FishConfig) objectConfig);
                        break;
                    case "PredatoryFish":
                        defaultConfig = defaultsDict.ContainsKey("PredatoryFish")
                            ? defaultsDict["PredatoryFish"]
                            : new FishConfig();
                        objectConfig = FishConfig.Fill(objJson, (FishConfig) defaultConfig);
                        obj = new PredatoryFish((FishConfig) objectConfig);
                        break;
                    default:
                        throw new ArgumentException("Object has no type");
                }

                objects.Add(obj);
            }

            return objects;
        }
    }
}