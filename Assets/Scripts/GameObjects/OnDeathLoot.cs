using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class OnDeathLoot : MonoBehaviour
    {
        [SerializeField]
        private int id;
        private void OnDeathAction()
        {
            Instantiate(Registry.Registry.СonstData.GameItems[id], transform.position, Quaternion.Euler(Vector3.zero));
        }
        private void Awake()
        {
            if(TryGetComponent(out HitPointSystem owner))
            {
                owner.OnDeathEvent += OnDeathAction;
            }
        }
        private void OnDestroy()
        {
            if (TryGetComponent(out HitPointSystem owner))
            {
                owner.OnDeathEvent -= OnDeathAction;
            }
        }
    }
}