using Newtonsoft.Json.Linq;

namespace AquariumSimulator.Model
{
    public abstract class CreatureConfig : AquaObjectConfig
    {
        public uint energyMax;
        
        protected static CreatureConfig Fill(JToken json, CreatureConfig config)
        {
            AquaObjectConfig.Fill(json, config);
            
            if (json["EnergyMax"] is not null)
                config.energyMax = (uint)json["EnergyMax"];

            return config;
        }
    }
    
    public abstract class Creature : AquaObject
    {
        protected readonly uint energyMax;
        protected uint energy;
        protected uint age;

        protected Creature(CreatureConfig config) : base(config)
        {
            energyMax = config.energyMax;
            energy = config.energyMax;
            age = 0;
        }

        public virtual uint BecomeEaten()
        {
            uint eaten = energy;
            energy = 0;
            return eaten;
        }
        
        public override void NextStage()
        {
            ++age;
        }
    }
}