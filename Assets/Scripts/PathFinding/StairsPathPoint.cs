using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class StairsPathPoint : PathPoint
    {
        public class StairsPathWay : PathWay
        {
            public StairsPathWay(PathPoint FirstPoint, PathPoint SecondPoint) : base(FirstPoint, SecondPoint,
                Vector2.Distance(FirstPoint.transform.position, SecondPoint.transform.position))
            { }
            public StairsPathWay(PathPointPair pair) : this(pair.First, pair.Second) { }
        }
    }
}
