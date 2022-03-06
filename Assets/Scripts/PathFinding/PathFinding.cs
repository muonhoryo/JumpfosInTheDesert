using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using Registry;

namespace PathFinding
{
    public static class PathFinding
    {
        public struct PathWayInfo
        {
            public PathWayInfo(Vector2 NextPoint,bool isStairs)
            {
                this.NextPoint = NextPoint;
                this.isStairs = isStairs;
            }
            public Vector2 NextPoint;
            public bool isStairs;
        }
        public sealed class DextraPathFinding
        {
            private Action Destruction = delegate { };
            ~DextraPathFinding()
            {
                Destruction();
            }
            private sealed class DextraPath
            {
                private DextraPath() { }
                public DextraPath(PathPoint start)
                {
                    Path = new List<PathPoint> { start };
                    Length = 0;
                }
                public DextraPath(List<PathPoint> Path,float Length)
                {
                    this.Path = Path;
                    this.Length = Length;
                }
                public DextraPath(DextraPath copyiedPath)
                {
                    Path = new List<PathPoint> { };
                    Path.AddRange(copyiedPath.Path);
                    Length = copyiedPath.Length;
                }
                public readonly List<PathPoint> Path;
                public PathPoint LastPoint => Path[Path.Count - 1];
                public readonly float Length;
            }
            private DextraPathFinding() { }
            public DextraPathFinding(PathPoint Start,PathPoint End)
            {
                this.End = End;
                Paths = new List<DextraPath>{new DextraPath(Start)};
            }
            private readonly PathPoint End;
            private PathPoint[] FinalPathWay=null;
            private readonly List<PathPoint> CheckedPathpoints = new List<PathPoint> { };
            private readonly List<DextraPath> Paths;
            private int LastPointIndex(PathPoint point)
            {
                for(int i = 0; i < Paths.Count; i++)
                {
                    if (Paths[i].LastPoint == point)
                    {
                        return i;
                    }
                }
                return -1;
            }
            private void CheckPoints()
            {
                {
                    Paths.Sort(delegate (DextraPath x, DextraPath y)
                    {
                        if (x.Length > y.Length)
                        {
                            return 1;
                        }
                        else if (x.Length == y.Length)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }
                    });
                    DextraPath[] pathsArray = Paths.ToArray();
                    foreach (var path in pathsArray)
                    {
                        if (path.LastPoint == End)
                        {
                            continue;
                        }
                        foreach (PathPoint.PathWay way in path.LastPoint.Ways)
                        {
                            PathPoint endPoint = way.FirstPoint == path.LastPoint ? way.SecondPoint : way.FirstPoint;
                            if (CheckedPathpoints.Contains(endPoint))
                            {
                                continue;
                            }
                            int index = LastPointIndex(endPoint);
                            if (index > -1)
                            {
                                float length = path.Length + way.Length;
                                if (length < Paths[index].Length)
                                {
                                    Paths[index] = new DextraPath(path);
                                    Paths[index].Path.Add(endPoint);
                                }
                            }
                            else
                            {
                                Paths.Add(new DextraPath(path));
                                Paths[Paths.Count - 1].Path.Add(endPoint);
                            }
                        }
                        Paths.Remove(path);
                        CheckedPathpoints.Add(path.LastPoint);
                    }
                }
                if (Paths.Count == 0)
                {
                    FinalPathWay = null;
                }
                else if (Paths.Count == 1 && Paths[0].LastPoint == End)
                {
                    FinalPathWay = Paths[0].Path.ToArray();
                }
                else
                {
                    CheckPoints();
                }
            }
            public void AsyncFindWay()
            {
                CheckPoints();
            }
            public Vector2[] GetPath()
            {
                ThreadManager.ThreadQueryReservator reservator = new ThreadManager.ThreadQueryReservator();
                void stopThreadQuery()
                {
                    Registry.Registry.ThreadManager.RemoveActionsQueve(reservator.Id);
                }
                Destruction += stopThreadQuery;
                Vector2[] path = FinalPathWay.AsyncConvertPath(reservator);
                Destruction -= stopThreadQuery;
                GC.Collect();
                return path;
            }
        }
    }
}