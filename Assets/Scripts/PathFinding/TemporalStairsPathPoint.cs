using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamingObjects;

namespace PathFinding
{
    public sealed class TemporalStairsPathPoint : StairsPathPoint, ITemporalPathPoint
    {
        private bool isInitialized = false;
        bool ITemporalPathPoint.isInitialized { get => isInitialized; set => isInitialized = value; }
        public Stairs Owner { private get; set; }
        ITempPointInstantiator ITemporalPathPoint.Owner { get => Owner; set => Owner = (Stairs)value; }
        public void Initialize()
        {
            TemporalPathPoint.Initialize(this);
        }
        public void CreateNewWay(PathPoint outterPoint)
        {
            new StairsPathWay(this, outterPoint);
        }
    }
}
