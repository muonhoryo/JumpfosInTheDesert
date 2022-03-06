using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class MeleeHitter : MonoBehaviour
    {
        [SerializeField]
        private HitPointSystem Owner;
        private readonly List<HitPointSystem> HittableSystems = new List<HitPointSystem> { };
        private System.Action<HitPointSystem> OnEnterAction = delegate { };
        public int Damage;
        public float HitDirection;
        public float HitForce;
        public float DefaultX{ get; private set; }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out HitPointSystem hittableSystem) && hittableSystem != Owner)
            {
                HittableSystems.Add(hittableSystem);
                OnEnterAction(hittableSystem);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.TryGetComponent(out HitPointSystem hittableSystem)&&hittableSystem!=Owner && HittableSystems.Contains(hittableSystem))
            {
                HittableSystems.Remove(hittableSystem);
            }
        }
        public void DamageAllSystems()
        {
            for(int i = 0; i < HittableSystems.Count; i++)
            {
                DamageSystem(HittableSystems[i]);
            }
        }
        private void DamageSystem(HitPointSystem system)
        {
            system.TakeDamage(new HitPointSystem.DamageHitInfo(Damage, HitDirection,Owner.Owner,HitForce));
        }
        private void Awake()
        {
            DefaultX = transform.localPosition.x;
            if (!enabled)
            {
                enabled = false;
            }
        }
        public void OnEnable()
        {
            OnEnterAction = DamageSystem;
            DamageAllSystems();
        }
        public void OnDisable()
        {
            OnEnterAction = delegate { };
        }
    }
}
