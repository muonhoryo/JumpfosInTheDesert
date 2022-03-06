using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class TestObjectHPSystem : TrashCanHPSystem
    {
        private void DamageMessage(int damage)
        {
            Debug.Log("Take " + damage + " damage");
        }
        public override void TakeDamage(DamageHitInfo hitInfo)
        {
            DamageMessage(hitInfo.HitDamage);
            RGBody.AddForce(hitInfo.HitDirectionVect2 * hitInfo.HitForce);
        }
    }
}
