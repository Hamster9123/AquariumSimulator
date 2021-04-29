using AquariumSimulator.Model.Space;


namespace AquariumSimulator.Model
{
    public class HerbivorousFish : Fish
    {
        public HerbivorousFish(FishConfig config) : base(config) {}

        public override uint BecomeEaten()
        {
            uint eaten = base.BecomeEaten();
            Die();
            return eaten;
        }
        
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
            
            return new HerbivorousFish(config);
        }
        
        public override void NextStage()
        {
            base.NextStage();
            if (!IsAlive) return;
            
            Point2D newLocation;

            if (Observer.FindNearest(Location, visionRange,
                o => o is PredatoryFish) is PredatoryFish predator)
            {
                newLocation = Observer.GetOppositeLocation(Location, predator.Location, speed);
                MoveTo(newLocation);
                return;
            }
            
            if (energy >= hungerLimit && 
                Observer.FindNearest(Location, visionRange,
                    o => o is HerbivorousFish {isMating: false, isPregnant: false} fish 
                         && fish.gender != gender 
                         && o != this) is HerbivorousFish partner)
            {
                newLocation = Observer.GetReachableLocation(Location, partner.Location, visionRange, speed);
                MoveTo(newLocation);
                
                if (Location == partner.Location)
                    Mate(partner);
                
                return;
            }
            
            if (Observer.FindNearest(Location, visionRange,
                o => o is Seaweed) is Seaweed prey)
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