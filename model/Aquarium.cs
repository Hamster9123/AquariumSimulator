using System;
using System.Collections.Generic;
using AquariumSimulator.Model.Space;


namespace AquariumSimulator.Model
{
    public class Aquarium
    {
        private readonly Space2D _space;
        private readonly List<AquaObject> _aquaObjects;
        private readonly List<PredatoryFish> _predators;
        private readonly List<HerbivorousFish> _herbivores;
        private readonly List<Seaweed> _seaweeds;
        private readonly List<Terrain> _terrains;
        
        public IEnumerable<AquaObject> AquaObjects => _aquaObjects;
        public IEnumerable<PredatoryFish> Predators => _predators;
        public IEnumerable<HerbivorousFish> Herbivores => _herbivores;
        public IEnumerable<Seaweed> Seaweeds => _seaweeds;
        public IEnumerable<Terrain> Terrains => _terrains;

        public uint Stage { get; private set; }
        public uint Width => _space.Width;
        public uint Height => _space.Height;
        public int NumObjects => _aquaObjects.Count;
        public int NumPredators => _predators.Count;
        public int NumHerbivores => _herbivores.Count;
        public int NumSeaweeds => _seaweeds.Count;
        public int NumStones => _terrains.Count;


        public Aquarium(uint width, uint height)
        {
            _space = new Space2D(width, height);
            _aquaObjects = new List<AquaObject>();
            _predators = new List<PredatoryFish>();
            _herbivores = new List<HerbivorousFish>();
            _seaweeds = new List<Seaweed>();
            _terrains = new List<Terrain>();
        }

        public void NextStage()
        {
            ++Stage;

            List<AquaObject> objs = new List<AquaObject>(_aquaObjects);
            foreach (AquaObject obj in objs)
            {
                if (obj.IsActive)
                    obj.NextStage();
            }
        }

        public void AddObject(AquaObject obj)
        {
            if (_aquaObjects.Contains(obj))
            {
                throw new ArgumentException("Aquarium already contains the object");
            }

            _space.AddObject(obj);
            _aquaObjects.Add(obj);

            switch (obj)
            {
                case PredatoryFish predatoryFish:
                    _predators.Add(predatoryFish);
                    break;
                case HerbivorousFish herbivorousFish:
                    _herbivores.Add(herbivorousFish);
                    break;
                case Seaweed seaweed:
                    _seaweeds.Add(seaweed);
                    break;
                case Terrain stone:
                    _terrains.Add(stone);
                    break;
            }
            
            obj.Aquarium = this;
            obj.Observer = _space.GetObserver();
        }

        public void RemoveObject(AquaObject obj)
        {
            if (!_aquaObjects.Contains(obj))
            {
                throw new ArgumentException("Aquarium doesn't contain the object");
            }
            
            _space.RemoveObject(obj);
            _aquaObjects.Remove(obj);
            
            switch (obj)
            {
                case PredatoryFish predatoryFish:
                    _predators.Remove(predatoryFish);
                    break;
                case HerbivorousFish herbivorousFish:
                    _herbivores.Remove(herbivorousFish);
                    break;
                case Seaweed seaweed:
                    _seaweeds.Remove(seaweed);
                    break;
                case Terrain stone:
                    _terrains.Remove(stone);
                    break;
            }
            
            obj.Aquarium = null;
            obj.Observer = null;
        }

        public void MoveObject(AquaObject obj, Point2D destination)
        {
            if (!_aquaObjects.Contains(obj))
            {
                throw new ArgumentException("Aquarium doesn't contain the object");
            }
            
            _space.MoveObject(obj, destination);
        }
    }
}
