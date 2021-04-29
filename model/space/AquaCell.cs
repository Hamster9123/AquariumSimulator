using System;
using System.Collections.Generic;
using System.Linq;


namespace AquariumSimulator.Model.Space
{
    public partial class Space2D
    {
        private class AquaCell
        {
            public Point2D Center { get; }
            public LinkedList<AquaObject> AquaObjects { get; }

            public AquaCell(Point2D center)
            {
                Center = center;
                AquaObjects = new LinkedList<AquaObject>();
            }

            public void AddObject(AquaObject aquaObject)
            {
                if (aquaObject is Terrain && AquaObjects.Count > 0)
                    throw new Exception("Terrain can't be added to the cell with objects");
                    
                if (AquaObjects.Any(o => o is Terrain))
                    throw new Exception("AquaObject can't be added to the cell with terrain");

                if (!AquaObjects.Contains(aquaObject))
                    AquaObjects.AddLast(aquaObject);
            }

            public void RemoveObject(AquaObject aquaObject)
            {
                if (!AquaObjects.Remove(aquaObject))
                    throw new Exception("Cell doesn't contain the object");
            }
        }
    }
}