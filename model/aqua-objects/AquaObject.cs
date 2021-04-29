using System;
using AquariumSimulator.Model.Space;
using Newtonsoft.Json.Linq;


namespace AquariumSimulator.Model
{
    public enum Gender
    {
        Male,
        Female
    }

    public abstract class AquaObjectConfig
    {
        public Point2D location;

        protected static AquaObjectConfig Fill(JToken json, AquaObjectConfig config)
        {
            if (config is null)
                throw new ArgumentException("Config is null");
            
            if (json["X"] is not null || json["Y"] is not null)
                config.location = new Point2D((int)json["X"], (int)json["Y"]);

            return config;
        }
    }
    
    public abstract class AquaObject
    {
        protected static readonly Random Random = new Random();
        public Aquarium Aquarium { get; set; }
        public ISpaceObserver Observer { protected get; set; }
        
        public Point2D Location { get; private set; }
        public bool IsActive { get; protected set; }
        
        public abstract void NextStage();

        protected AquaObject(AquaObjectConfig config)
        {
            Location = config.location;
            IsActive = true;
        }

        protected void MoveTo(Point2D destination)
        {
            Aquarium.MoveObject(this, destination);
            Location = destination;
        }
        
    }
}