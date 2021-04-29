using AquariumSimulator.Model.Space;

namespace AquariumSimulator.Model
{
    public class PredatoryFish : Fish
    {
        public PredatoryFish(FishConfig config) : base(config)  {}
        
        protected override Fish GiveBirth()
        {
            var config = new FishConfig {
                ageMaturity = this.ageMaturity,
                ageOld = this.ageOld,
                ageDeath = this.ageDeath,
                hungerLimit = this.hungerLimit,
                visionRange = this.visionRange,
                speed = this.speed,
                pregnancyEnd = this.pregnancyEnd,
                metabolism = this.metabolism,
                gender = Random.Next(0, 2) == 0 ? Gender.Female : Gender.Male,
                energyMax = this.energyMax
            };
            
            return new PredatoryFish(config);
        }

        public override void NextStage()
        {
            base.NextStage();
            if (!IsAlive) return;
            
            Point2D newLocation;

            if (energy >= hungerLimit && 
                Observer.FindNearest(Location, visionRange,
                    o => o is PredatoryFish {isMating: false, isPregnant: false} fish 
                         && fish.gender != gender 
                         && o != this) is PredatoryFish partner)
            {
                newLocation = Observer.GetReachableLocation(Location, partner.Location, visionRange, speed);
                MoveTo(newLocation);
                
                if (Location == partner.Location)
                    Mate(partner);
                
                return;
            }
            
            if (Observer.FindNearest(Location, visionRange,
                o => o is HerbivorousFish) is HerbivorousFish prey)
            {
                newLocation = Observer.GetReachableLocation(Location, prey.Location, visionRange, speed);
                MoveTo(newLocation);
                
                if (Location != prey.Location)
                    Eat(prey);
                
                return;
            }
            
            newLocation = Observer.GetRandomLocation(Location, speed);
            MoveTo(newLocation);
        }
    }
}