using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

namespace GamingObjects
{
    public sealed class Stairs : Platform<TemporalStairsPathPoint>
    {
        [SerializeField]
        private float TiltAngle;
        [SerializeField]
        private StairsPathPoint upperPoint;
        [SerializeField]
        private StairsPathPoint lowerPoint;
        public StairsPathPoint UpperPoint { get => upperPoint; }
        public StairsPathPoint LowerPoint { get => lowerPoint; }
        public override PathPoint FirstPoint => upperPoint;
        public override PathPoint SecondPoint => lowerPoint;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.collider.TryGetComponent(out GameCharacter owner))
            {
                owner.SetGravityScale(0);
                owner.SetMoveForceDirection(TiltAngle.Direction());
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if(collision.collider.TryGetComponent(out GameCharacter owner))
            {
                owner.SetGravityScale();
                owner.SetMoveForceDirection();
            }
        }
        public override ITemporalPathPoint InstantiateTempPoint(Vector2 position)
        {
            return InstantiateTempPoint(new Vector2(position.x,position.x*Mathf.Tan(TiltAngle)+transform.position.y),
                Registry.Registry.ÑonstData.TempStairsPathPointPrefab,this);
        }
    }
}
