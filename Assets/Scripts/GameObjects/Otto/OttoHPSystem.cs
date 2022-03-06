using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class OttoHPSystem : HitPointSystem
    {
        [SerializeField]
        private Otto OttoScript;
        public override GameCharacter Owner => OttoScript;
    }
}
