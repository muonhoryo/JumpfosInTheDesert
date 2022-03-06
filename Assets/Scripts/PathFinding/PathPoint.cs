using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class PathPoint : MonoBehaviour
    {
        public class PathWay
        {
            private PathWay() { }
            protected PathWay(PathPoint FirstPoint, PathPoint SecondPoint, float Length)
            {
                this.FirstPoint = FirstPoint;
                this.SecondPoint = SecondPoint;
                FirstPoint.AddWay(this);
                SecondPoint.AddWay(this);
                this.Length = Length;
            }
            public PathWay(PathPoint FirstPoint, PathPoint SecondPoint) : this(FirstPoint, SecondPoint,
                Mathf.Abs(FirstPoint.xPos - SecondPoint.xPos))
            { }
            public PathWay(PathPointPair pair) : this(pair.First, pair.Second) { }
            public readonly float Length;
            public readonly PathPoint FirstPoint;
            public readonly PathPoint SecondPoint;
        }
        public float Level => transform.position.y;
        public float xPos => transform.position.x;
        public PathWay[] Ways { get; protected set; } = new PathWay[] { };
        public bool isContainsWay(PathWay way)
        {
            foreach(PathWay pathWay in Ways)
            {
                if (pathWay.FirstPoint == way.FirstPoint)
                {
                    if (pathWay.SecondPoint == way.SecondPoint)
                    {
                        return true;
                    }
                }
                else if (pathWay.SecondPoint == way.FirstPoint)
                {
                    if (pathWay.FirstPoint == way.SecondPoint)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        protected void AddWay(PathWay way)
        {
            List<PathWay> list = new List<PathWay> { };
            list.AddRange(Ways);
            list.Add(way);
            Ways = list.ToArray();
        }
        protected void RemoveWay(PathWay way)
        {
            List<PathWay> list = new List<PathWay> { };
            foreach(PathWay pathWay in Ways)
            {
                if (pathWay != way)
                {
                    list.Add(pathWay);
                }
            }
            Ways = list.ToArray();
        }
        private void OnDestroy()
        {
            foreach(PathWay way in Ways)
            {
                void removeWay(PathPoint targetPoint)
                {
                    targetPoint.RemoveWay(way);
                }
                if (way.FirstPoint == this)
                {
                    removeWay(way.SecondPoint);
                }
                else
                {
                    removeWay(way.FirstPoint);
                }
            }
        }
    }
}
