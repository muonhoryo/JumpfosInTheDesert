using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    [CreateAssetMenu]
    public class CharacterSettings : ScriptableObject
    {
        [Range(0, 1000000)]
        public float WalkSpeed;
        [Range(0,31)]
        public int DefaultLayer;
        [Range(0, 31)]
        public int IgnoringHalfPassLayer;
    }
}
