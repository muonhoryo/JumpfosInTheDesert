using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    [CreateAssetMenu]
    public sealed class MainHeroSettings : RunningCharacterSettings
    {
        public Vector2 PullOverCheckBoxSize;
        [Range(0, 10)]
        public float JumpingLockDelay;
        [Range(0, 1000000)]
        public float JumpForce;
        [Range(0, 1000000)]
        public float JumpForceModifier;
        [Range(0, 1000000)]
        public float SecondJumpForce;
        [Range(0, 1000000)]
        public float WallJumpForce;
        [Range(0,360)]
        public float WallJumpAngle;
        [Range(0, 10)]
        public float DownActionDelay;
        [Range(0, 5)]
        public float MaxFirstJumpLevel;
        [Range(0, 10)]
        public float JumpLandingDelay;
        [Range(0, 10000)]
        public float OnFallingMoveSpeed;
        [Range(0, 10000)]
        public float PullOverSpeed;
        [Range(0,10000)]
        public float OnFallKickStartGravity;
        [Range(-10000,10000)]
        public float OnWallSlideGravity;
        [Range(0, 100)]
        public float PullOverCheckBoxOffsetX;
    }
}
