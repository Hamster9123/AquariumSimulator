using System;
using System.Collections.Generic;
using System.Linq;


namespace AquariumSimulator.Model.Space
{
    public interface ISpaceObserver
    {
        uint Width { get; }
        uint Height { get; }

        IEnumerable<AquaObject> ObjectsInRange(Point2D center, uint range);

        AquaObject FindNearest(Point2D center, uint range, Func<AquaObject, bool> predicate);

        Point2D GetReachableLocation(Point2D from, Point2D to, uint observationRange, uint maxDistance);

        Point2D GetRandomLocation(Point2D location, uint range);

        Point2D GetOppositeLocation(Point2D location, Point2D from, uint range);
    }
    
    
    public partial class Space2D
    {
        private class SpaceObserver : ISpaceObserver
        {
            private static readonly Random Random = new Random();

            private readonly Space2D _space2D;

            public uint Width { get; }
            public uint Height { get; }


            public SpaceObserver(Space2D space2D)
            {
                _space2D = space2D;
                Width = space2D.Width;
                Height = space2D.Height;
            }

            public IEnumerable<AquaObject> ObjectsInRange(Point2D center, uint range)
            {
                var objects = new List<AquaObject>();
                int startX = (int) Math.Max(0, center.X - range);
                int startY = (int) Math.Max(0, center.Y - range);
                int endX = (int) Math.Min(Width - 1, center.X + range);
                int endY = (int) Math.Min(Height - 1, center.Y + range);

                for (int x = startX; x <= endX; ++x)
                for (int y = startY; y <= endY; ++y)
                    objects.AddRange(_space2D.GetObjectsInCell(x, y));

                return objects;
            }

            public AquaObject FindNearest(Point2D center, uint range, Func<AquaObject, bool> predicate)
            {
                IEnumerable<AquaObject> objectsInRange = ObjectsInRange(center, range).Where(predicate);
                AquaObject nearest = null;
                double minDistance = double.MaxValue;

                foreach (AquaObject obj in objectsInRange)
                {
                    double distance = Math.Sqrt(Math.Pow(center.X - obj.Location.X, 2) +
                                                Math.Pow(center.Y - obj.Location.Y, 2));

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = obj;
                    }
                }

                return nearest;
            }

            
            public Point2D GetReachableLocation(Point2D from, Point2D to, uint observationRange, uint maxDistance)
            {
                if (from == to)
                {
                    return to;
                }
                
                // The visionRange region coordinates in the Aquarium
                int startX = Math.Max(0, from.X - (int)observationRange);
                int startY = Math.Max(0, from.Y - (int)observationRange);
                int endX = (int) Math.Min(Width - 1, from.X + observationRange);
                int endY = (int) Math.Min(Height - 1, from.Y + observationRange);

                // The size of the region of the vision
                int width = endX - startX + 1;
                int height = endY - startY + 1;

                const int wall = -1;
                const int blank = -2;
                int d;
                int x, y;

                #region Fill the visionRegion array with wall and blank values

                int[,] visionRegion = new int[width, height];
                for (x = startX; x <= endX; ++x)
                for (y = startY; y <= endY; ++y)
                    if (_space2D.GetObjectsInCell(x, y).Any(o => o is Terrain))
                        visionRegion[x - startX, y - startY] = wall;
                    else
                        visionRegion[x - startX, y - startY] = blank;

                #endregion

                #region Lee algorithm

                var queue = new Queue<int[]>();
                var shifts = new int[8, 2]
                {
                    {-1, -1}, {-1, 0}, {-1, 1},
                    {0, -1}, {0, 1},
                    {1, -1}, {1, 0}, {1, 1}
                };

                // Initialization
                visionRegion[from.X - startX, from.Y - startY] = 0;
                queue.Enqueue(new int[2] {from.X - startX, from.Y - startY});

                // Wave expansion
                while (queue.Count > 0)
                {
                    var coord = queue.Dequeue();
                    d = visionRegion[coord[0], coord[1]] + 1;
                    for (int i = 0; i < 8; ++i)
                    {
                        int dx = shifts[i, 0], dy = shifts[i, 1];
                        x = coord[0] + dx;
                        y = coord[1] + dy;
                        if (!(x < 0 || x >= width || y < 0 || y >= height) && (visionRegion[x, y] == blank))
                        {
                            visionRegion[x, y] = d;
                            queue.Enqueue(new int[2] {x, y});
                        }
                    }
                }

                // Select wishedDestination cell
                var path = new List<int[]>();
                int dstX = to.X - startX;
                int dstY = to.Y - startY;

                if (visionRegion[dstX, dstY] == blank || visionRegion[dstX, dstY] == wall)
                {
                    bool dstIsFound = false;
                    queue.Enqueue(new int[2] {dstX, dstY});
                    while (queue.Count > 0 && !dstIsFound)
                    {
                        var coord = queue.Dequeue();
                        d = visionRegion[coord[0], coord[1]] + 1;
                        for (int i = 0; i < 8; ++i)
                        {
                            int dx = shifts[i, 0], dy = shifts[i, 1];
                            x = coord[0] + dx;
                            y = coord[1] + dy;
                            if (x < 0 || x >= width || y < 0 || y >= height) continue;
                            if (visionRegion[x, y] > 0)
                            {
                                dstX = x;
                                dstY = y;
                                dstIsFound = true;
                            }
                            else
                            {
                                queue.Enqueue(new int[2] {x, y});
                            }
                        }
                    }
                }

                // Backtrace
                d = visionRegion[dstX, dstY] - 1;
                int curX = dstX;
                int curY = dstY;
                while (d >= 0)
                {
                    path.Add(new int[2] {curX, curY});
                    for (int i = 0; i < 8; ++i)
                    {
                        int dx = shifts[i, 0], dy = shifts[i, 1];
                        x = curX + dx;
                        y = curY + dy;
                        if (!(x < 0 || x >= width || y < 0 || y >= height) && (visionRegion[x, y] == d))
                        {
                            curX = x;
                            curY = y;
                            break;
                        }
                    }

                    --d;
                }

                int dstIdx = Math.Max(0, path.Count - (int) maxDistance);
                x = path[dstIdx][0];
                y = path[dstIdx][1];
                Point2D reachableLocation = new Point2D((int) (x + startX), (int) (y + startY));

                #endregion

                return reachableLocation;
            }

            public Point2D GetRandomLocation(Point2D location, uint range)
            {
                Point2D dst;
                do
                {
                    int xShift = Random.Next(-(int) range, (int) range + 1);
                    int yShift = Random.Next(-(int) range, (int) range + 1);

                    int x = (int) Math.Min(Math.Max((long) location.X + xShift, 0), Width - 1);
                    int y = (int) Math.Min(Math.Max((long) location.Y + yShift, 0), Height - 1);
                    dst = new Point2D(x, y);
                        
                } while (_space2D.GetObjectsInCell(dst.X, dst.Y).Any(o => o is Terrain));

                return dst;
            }

            public Point2D GetOppositeLocation(Point2D location, Point2D from, uint range)
            {
                double x, y;
                if (from.X != location.X)
                {
                    double k = ((double) from.Y - location.Y) / ((double) from.X - location.X);
                    double c = location.X * location.X - (double) range * range / (1 + k * k);
                    double sqrtD = Math.Sqrt(location.X * location.X - c);
                    double x1 = location.X - sqrtD;
                    double x2 = location.X + sqrtD;
                    x = location.X < from.X ? x1 : x2;
                    y = k * (x - location.X) + location.Y;
                }
                else
                {
                    x = location.X;
                    y = location.Y + (location.Y < from.Y ? -range : range);
                }

                x = Math.Min(Math.Max(x, 0), Width - 1);
                y = Math.Min(Math.Max(y, 0), Height - 1);
                Point2D dst = new Point2D((int) x, (int) y);

                if (_space2D.GetObjectsInCell(dst.X, dst.Y).Any(o => o is Terrain))
                    dst = GetRandomLocation(location, range);
                return dst;
            }
        }
    }
}