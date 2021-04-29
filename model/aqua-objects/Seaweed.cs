using Newtonsoft.Json.Linq;

namespace AquariumSimulator.Model
{
    public class SeaweedConfig : CreatureConfig
    {
        public uint energyIncome;
        
        public static SeaweedConfig Fill(JToken json, SeaweedConfig config)
        {
            CreatureConfig.Fill(json, config);
            
            if (json["EnergyIncome"] is not null)
                config.energyIncome = (uint)json["EnergyIncome"];

            return config;
        }
    }
    
    public class Seaweed : Creature
    {
        protected readonly uint energyIncome;

        public Seaweed(SeaweedConfig config) : base(config)
        {
            energyIncome = config.energyIncome; 
        }

        private void Grow()
        {
            if (energy + energyIncome > energyMax)
                energy = energyMax;
            else energy += energyIncome;
        }

        public override void NextStage()
        {
            base.NextStage();
            Grow();
        }
    }
}