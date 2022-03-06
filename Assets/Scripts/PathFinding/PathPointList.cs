using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public struct PathPointPair
    {
        public PathPointPair(PathPoint First,PathPoint Second)
        {
            this.First = First;
            this.Second = Second;
        }
        public PathPoint First;
        public PathPoint Second;
        public static PathPointPair[] ConvertPointsArrays(PathPoint[] Starts, PathPoint[] Ends)
        {
            PathPointPair[] connections = new PathPointPair[Mathf.Min(Starts.Length, Ends.Length)];
            for(int i = 0; i < connections.Length; i++)
            {
                connections[i] = new PathPointPair(Starts[i], Ends[i]);
            }
            return connections;
        }
    } 
    public sealed class PathPointList : MonoBehaviour
    {
        public PathPoint[] Points = new PathPoint[] { };
        public PathPoint[] StartSimplePoints = new PathPoint[] { };
        public PathPoint[] EndSimplePoints = new PathPoint[] { };
        public PathPoint[] StartStairsPoints = new PathPoint[] { };
        public PathPoint[] EndStairsPoints = new PathPoint[] { };
    }
}
