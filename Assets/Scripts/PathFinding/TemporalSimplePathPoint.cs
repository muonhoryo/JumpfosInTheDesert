using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamingObjects;

namespace PathFinding
{
    public sealed class TemporalSimplePathPoint : PathPoint,ITemporalPathPoint
    {
        private bool isInitialized = false;
        bool ITemporalPathPoint.isInitialized { get=> isInitialized; set=> isInitialized=value; }
        public SimplePlatform Owner { private get; set; }
        ITempPointInstantiator ITemporalPathPoint.Owner { get => Owner; set => Owner =(SimplePlatform)value; }
        public void Initialize()
        {
            TemporalPathPoint.Initialize(this);
        }
        public void CreateNewWay(PathPoint outterPoint)
        {
            new PathWay(this, outterPoint);
        }
    }
}
