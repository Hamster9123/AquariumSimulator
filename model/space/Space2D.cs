using System.Collections.Generic;


namespace AquariumSimulator.Model.Space
{
    public partial class Space2D
    {
        private readonly AquaCell[,] _cells;
        public uint Width { get; }
        public uint Height { get; }

        public Space2D(uint width, uint height)
        {
            Width = width;
            Height = height;
            _cells = new AquaCell[Width, Height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    _cells[x, y] = new AquaCell(new Point2D(x, y));
                }
            }
        }

        public void AddObject(AquaObject obj)
        {
            _cells[obj.Location.X, obj.Location.Y].AddObject(obj);
        }

        public void RemoveObject(AquaObject obj)
        {
            _cells[obj.Location.X, obj.Location.Y].RemoveObject(obj);
        }

        public void MoveObject(AquaObject obj, Point2D destination)
        {
            _cells[obj.Location.X, obj.Location.Y].RemoveObject(obj);
            _cells[destination.X, destination.Y].AddObject(obj);
        }

        public ISpaceObserver GetObserver()
        {
            return new SpaceObserver(this);
        }

        public IEnumerable<AquaObject> GetObjectsInCell(int x, int y)
        {
            return _cells[x, y].AquaObjects;
        }
    }
}