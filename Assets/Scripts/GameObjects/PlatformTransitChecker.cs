using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class PlatformTransitChecker : MonoBehaviour
    {
        private StepChecker Owner;
        private GameCharacter CheckableObject;
        public void Initialize(StepChecker Owner,GameCharacter CheckableObject)
        {
            this.Owner = Owner;
            this.CheckableObject = CheckableObject;
        }
        private void FixedUpdate()
        {
            if (CheckableObject.VerticalVelocity <= 0)
            {
                Owner.TransitIsDone();
            }
        }
    }
}
