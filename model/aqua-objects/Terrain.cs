using Newtonsoft.Json.Linq;

namespace AquariumSimulator.Model
{
    public class TerrainConfig : AquaObjectConfig
    {
        public static TerrainConfig Fill(JToken json, TerrainConfig config)
        {
            CreatureConfig.Fill(json, config);
            return config;
        }
    }
    
    public class Terrain : AquaObject
    {
        public Terrain(TerrainConfig config) : base(config) {}
        
        public override void NextStage() {}
    }
}