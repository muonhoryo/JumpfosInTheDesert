using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public class Otto : RunningEnemy
    {
        [SerializeField]
        private OttoHPSystem OttoHPSystem;
        public override HitPointSystem HitPointSystem => OttoHPSystem;
        private static readonly Func<float>[] moveSpeedConsts = new Func<float>[2]
        {
            ()=>Registry.Registry.OttoSettings.WalkSpeed,
            ()=>Registry.Registry.OttoSettings.RunSpeed
        };
        protected override Func<float>[] MoveSpeedConsts => moveSpeedConsts;
        public override CharacterSettings ConstData =>Registry.Registry.OttoSettings;
        public override void TakeDamage(HitPointSystem.DamageHitInfo hitInfo)
        {
            base.TakeDamage(hitInfo);
            HorizontalDirection = hitInfo.HitDirectionVect2.x > 0 ? 1 : -1;
        }
        private void Start()
        {
            moveSpeed = Registry.Registry.OttoSettings.WalkSpeed;
        }
        public void SetPassability(bool isIgnoreHalfMovebleLayers)
        {
            IgnoringHalfMovebleLayers = isIgnoreHalfMovebleLayers;
        }
    }
}
