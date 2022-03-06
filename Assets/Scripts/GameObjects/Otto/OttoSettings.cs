using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    [CreateAssetMenu]
    public class OttoSettings : RunningCharacterSettings
    {
        [Range(0,1000)]
        public float PatrulDelayTime;
    }
}
