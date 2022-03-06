using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public class TrashCanHPSystem : HitPointSystem
    {
        [SerializeField]
        protected Rigidbody2D RGBody;
        public override GameCharacter Owner => null;
        public override void TakeDamage(DamageHitInfo hitInfo)
        {
            RGBody.AddForce(hitInfo.HitDirectionVect2 * hitInfo.HitForce);
            base.TakeDamage(hitInfo);
        }
    }
}