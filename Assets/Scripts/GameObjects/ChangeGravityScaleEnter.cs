using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public class ChangeGravityScaleEnter : StateMachineBehaviour
    {
        public float GravityScale;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<GameCharacter>().SetGravityScale(GravityScale);
        }
    }
}
