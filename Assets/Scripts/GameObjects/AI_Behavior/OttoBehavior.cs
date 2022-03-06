using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

namespace GamingObjects
{
    public sealed partial class OttoBehavior : AIBehavior<OttoBehavior,Otto>
    {
        public sealed class OttoStateConverter : AIStateConverter
        {
            public OttoStateConverter(OttoBehavior behavior)
            {
                this.behavior = behavior;
            }
            private readonly OttoBehavior behavior;
            public override Action UpdateAction => delegate { behavior.currentState.UpdateAction(behavior); };
            public override Action<HitPointSystem.DamageHitInfo> OnTakeDamage => 
                delegate(HitPointSystem.DamageHitInfo hitInfo) { behavior.currentState.OnTakeDamage( behavior,hitInfo); };
            public override Action<GameCharacter> OnFindEnemy =>
                delegate(GameCharacter target) { behavior.currentState.OnFindEnemy(behavior, target); };
            public override Action OnLostEnemy => delegate { behavior.currentState.OnLostEnemy(behavior); };
            public override Action OnAnimationEnd => delegate { behavior.currentState.OnAnimationEnd(behavior); };
            public override Action OnEnterAction => delegate { behavior.currentState.OnEnterAction(behavior); };
            public override Action OnExitAction => delegate { behavior.currentState.OnExitAction(behavior); };
            public override string Name => behavior.currentState.Name;
        }
        IEnumerator PatrulDelay()
        {
            yield return new WaitForSeconds(Registry.Registry.OttoSettings.PatrulDelayTime);
            ChangeBehaviorState(PatrulMode);
            yield break;
        }
        private Coroutine CurrentPatrulDelay;
        [SerializeField]
        private Vector2[] PatrulPoints;
        [SerializeReference]
        private Vector2[] CurrentPath;
        [SerializeReference]
        private int NextPathPoint=-1;
        [SerializeReference]
        private int NextPatrulPoint=-1;
        [SerializeField]
        private Otto OttoScript;
        protected override Otto Owner => OttoScript;
        [SerializeField]
        private GameCharacter HuntedTarget;
        private OttoStateConverter StateConverter;
        public override AIStateConverter ConvertedState => StateConverter;
        public override void OnFallingAction()
        {
            if(currentState is SubState<OttoBehavior, Otto>)
            {
                ChangeSubState(new OttoFallingState((currentState as SubState<OttoBehavior, Otto>).NextState));
            }
            else
            {
                ChangeSubState(new OttoFallingState(currentState));
            }
        }
        private void Start()
        {
            if (!OttoScript.isFalling)
            {
                SetStartState(PatrulMode);
            }
            else
            {
                SetStartSubState(new OttoFallingState(PatrulMode));
            }
        }
        protected override void Awake()
        {
            base.Awake();
            StateConverter = new OttoStateConverter(this);
            OttoScript.OnTakeDamageEvent += ConvertedState.OnTakeDamage;
            OttoScript.OnAnimationEndEvent += ConvertedState.OnAnimationEnd;
        }
    }
}
