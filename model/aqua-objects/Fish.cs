using System;
using Newtonsoft.Json.Linq;


namespace AquariumSimulator.Model
{
    public class FishConfig : CreatureConfig
    {            
        public uint ageMaturity;
        public uint ageOld;
        public uint ageDeath;
        public uint hungerLimit;
        public uint energyDecrease;
        public uint visionRange;
        public uint speed;
        public uint pregnancyEnd;
        public float metabolism;
        public Gender gender;

        public static FishConfig Fill(JToken json, FishConfig config)
        {
            CreatureConfig.Fill(json, config);
            
            if (json["AgeMaturity"] is not null)
                config.ageMaturity = (uint)json["AgeMaturity"];
            if (json["AgeOld"] is not null)
                config.ageOld = (uint)json["AgeOld"];
            if (json["AgeDeath"] is not null)
                config.ageDeath = (uint)json["AgeDeath"];
            if (json["HungerLimit"] is not null)
                config.hungerLimit = (uint)json["HungerLimit"];
            if (json["EnergyDecrease"] is not null)
                config.energyDecrease = (uint)json["EnergyDecrease"];
            if (json["VisionRange"] is not null)
                config.visionRange = (uint)json["VisionRange"];
            if (json["Speed"] is not null)
                config.speed = (uint)json["Speed"];
            if (json["PregnancyEnd"] is not null)
                config.pregnancyEnd = (uint)json["PregnancyEnd"];
            if (json["Metabolism"] is not null)
                config.metabolism = (float)json["Metabolism"];
            if (json["Gender"] is not null)
                config.gender = json["Gender"].ToString() == "Male" ? Gender.Male : Gender.Female;

            return config;
        }
    }
    
    public abstract class Fish : Creature
    {
        protected readonly uint ageMaturity;
        protected readonly uint ageOld;
        protected readonly uint ageDeath;
        protected readonly uint hungerLimit;
        protected readonly uint energyDecrease;
        protected readonly uint visionRange;
        protected readonly uint speed;
        protected readonly uint pregnancyEnd;
        protected readonly float metabolism;
        protected readonly Gender gender;
        
        protected uint pregnancyTime;
        protected bool isMating;
        protected bool isPregnant;
        public bool IsAlive { get; private set; }

        protected Fish(FishConfig config) : base(config)
        {
            if (config.ageMaturity >= config.ageOld)
                throw new ArgumentException("Age of maturity >= old age");
            if (config.ageOld >= config.ageDeath)
                throw new ArgumentException("Old age >= age of death"); 
            if (config.speed > config.visionRange)
                throw new ArgumentException("Speed > vision range");
            if (config.metabolism is <= 0 or > 1)
                throw new ArgumentException("Metabolism not in (0, 1]");
                        
            ageMaturity = config.ageMaturity;
            ageOld = config.ageOld;
            ageDeath = config.ageDeath;
            hungerLimit = config.hungerLimit;
            energyDecrease = config.energyDecrease;
            visionRange = config.visionRange;
            speed = config.speed;
            pregnancyEnd = config.pregnancyEnd;
            metabolism = config.metabolism;
            gender = config.gender;
            pregnancyTime = 0;
            isMating = false;
            isPregnant = false;
            IsAlive = true;
        }

        protected void Eat(Creature creature)
        {
            uint energyIncome = (uint)(creature.BecomeEaten() * metabolism);

            if (energy + energyIncome > energyMax)
                energy = energyMax;
            else 
                energy += energyIncome;
        }

        protected void Mate(Fish fish)
        {
            isMating = true;
            fish?.Mate(null);

            if (gender == Gender.Female && !isPregnant)
                BecomePregnant();
        }

        protected void BecomePregnant()
        {
            if (age >= ageMaturity && age < ageOld)
                isPregnant = true;
        }

        protected void Die()
        {
            IsAlive = false;
            IsActive = false;
            Aquarium.RemoveObject(this);
        }

        public override void NextStage()
        {
            base.NextStage();
            
            if (!IsAlive) return;

            if (age >= ageDeath)
            {
                Die();
                return;
            }

            if (isPregnant)
            {
                if (pregnancyTime < pregnancyEnd)
                    ++pregnancyTime;
                else
                {
                    isPregnant = false;
                    pregnancyTime = 0;
                    Fish child = GiveBirth();
                    Aquarium.AddObject(child);
                }
            }

            if (isMating)
                isMating = false;

            energy = (uint)Math.Max(0, (long)energy - energyDecrease);
            if (energy == 0)
                Die();
        }
        
        protected abstract Fish GiveBirth();
    }
}