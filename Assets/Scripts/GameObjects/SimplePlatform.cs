using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

namespace GamingObjects
{
    public class SimplePlatform : Platform<TemporalSimplePathPoint>
    {
        [SerializeField]
        private PathPoint firstPoint;
        [SerializeField]
        private PathPoint secondPoint;
        public override PathPoint FirstPoint { get=> firstPoint; }
        public override PathPoint SecondPoint { get => secondPoint; }
        public float Level { get => firstPoint.Level; }
        public override ITemporalPathPoint InstantiateTempPoint(Vector2 position)
        {
            return InstantiateTempPoint(new Vector2(position.x,Level),
                Registry.Registry.ÑonstData.TempSimplePathPointPrefab,this);
        }
    }
}
