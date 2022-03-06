using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PathFinding
{
    public sealed class PathMap
    {
        bool isConstructed = false;
        public event Action OnChangeMapEvent = delegate { };
        public void OnMapReselected()
        {
            OnChangeMapEvent();
        }
        public PathMap()
        {
            Points = new Dictionary<float, PathPoint[]> { };
        }
        public PathMap(Dictionary<float,PathPoint[]> Points)
        {
            this.Points = Points;
        }
        public PathMap(List<PathPoint> pathPoints) : this(pathPoints.ToArray()) { }
        public PathMap(PathPoint[] pathPoints):this()
        {
            foreach (PathPoint point in pathPoints)
            {
                AddPoint(point);
            }
        }
        private readonly Dictionary<float,PathPoint[]> Points;
        public void AddPoint(PathPoint point)
        {
            OnChangeMapEvent();
            if (!Points.ContainsKey(point.Level))
            {
                Points.Add(point.Level, new PathPoint[1] { point });
            }
            else
            {
                PathPoint[] array = new PathPoint[Points[point.Level].Length + 1];
                Points[point.Level].CopyTo(array, 0);
                array[array.Length - 1] = point;
                Points[point.Level] = array;
                GC.Collect();
            }
        }
        public void RemovePoint(PathPoint point)
        {
            OnChangeMapEvent();
            if (Points[point.Level].Length == 1)
            {
                Points.Remove(point.Level);
            }
            else
            {
                PathPoint[] array = new PathPoint[Points[point.Level].Length - 1];
                Points[point.Level].CopyTo(array, 0);
                Points[point.Level] = array;
                GC.Collect();
            }
        }
        public PathPoint[] GetPointsAtLevel(float level)
        {
            PathPoint[] array;
            {
                List<PathPoint> list = new List<PathPoint> { };
                if (Points.ContainsKey(level))
                {
                    list.AddRange(Points[level]);
                    array = list.ToArray();
                }
                else
                {
                    float minDiff = float.MaxValue;
                    float nearestLevel = float.MaxValue;
                    foreach(var keyValue in Points)
                    {
                        float diff = Math.Abs(keyValue.Key - level);
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            nearestLevel = keyValue.Key;
                        }
                    }
                    list.AddRange(Points[nearestLevel]);
                    array = list.ToArray();
                }
            }
            return array;
        }
        public void ConstructPathWays(PathPointPair[] SimplePointPairs,PathPointPair[]StairsPointPairs)
        {
            if (!isConstructed)
            {
                for (int i = 0; i < SimplePointPairs.Length; i++)
                {
                    new PathPoint.PathWay(SimplePointPairs[i]);
                }
                for (int i = 0; i < StairsPointPairs.Length; i++)
                {
                    new StairsPathPoint.StairsPathWay(StairsPointPairs[i]);
                }
                isConstructed = true;
            }
        }
    }
}
