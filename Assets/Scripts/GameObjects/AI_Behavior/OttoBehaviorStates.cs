using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;


namespace GamingObjects
{
    public sealed partial class OttoBehavior
    {
        private static class UpdateActions
        {
            public static void BaseMoveToTarget(OttoBehavior behavior, Vector2 target, float endDistance, Action onMoveEndAction)
            {
                if (Vector2.Distance(behavior.transform.position, target) <= endDistance)
                {
                    onMoveEndAction();
                }
                else
                {

                    behavior.OttoScript.AnimatedMove(Math.Sign(target.x - behavior.transform.position.x));
                }
            }
            public static void PatrulUpdate(OttoBehavior behavior)
            {
                BaseMoveToTarget(behavior, behavior.PatrulPoints[behavior.NextPatrulPoint],
                    Registry.Registry.СonstData.MovePointEndDistance, delegate
                    { behavior.ChangeBehaviorState(PatrulDelayMode); });
            }
            public static void HuntingUpdate(OttoBehavior behavior)
            {
                BaseMoveToTarget(behavior, behavior.HuntedTarget.transform.position,
                    Registry.Registry.OttoSettings.MeleeAtkDistance, delegate
                    { behavior.ChangeBehaviorState(PatrulDelayMode); });
            }
        }
        private static class TakeDamageEvents
        {
            public static void OnPeacefulDamage(OttoBehavior behavior, HitPointSystem.DamageHitInfo hitInfo)
            {
                behavior.HuntedTarget = hitInfo.Owner;
                TakeDamage(behavior, hitInfo);
            }
            public static void TakeDamage(OttoBehavior behavior, HitPointSystem.DamageHitInfo hitInfo)
            {
                behavior.ChangeBehaviorState(HurtingMode);
                ReTakeDamage(behavior, hitInfo);
            }
            public static void ReTakeDamage(OttoBehavior behavior, HitPointSystem.DamageHitInfo hitInfo)
            {
                behavior.Owner.TakeDamage(hitInfo);
            }
        }
        private static class FindEnemyEvents
        {
            public static void OnPeacefulFind(OttoBehavior behavior, GameCharacter target)
            {
                behavior.HuntedTarget = target;
                behavior.ChangeBehaviorState(HuntingMode);
            }
        }
        private static class OtherMethods
        {
            public static void SetDefaultState(OttoBehavior behavior)
            {
                behavior.ChangeBehaviorState(PatrulMode);
            }
        }
        private static class StateActions
        {
            public static void PatrulEnter(OttoBehavior behavior)
            {
                if (behavior.PatrulPoints.Length == 0)
                {
                    behavior.ChangeBehaviorState(IdleMode);
                    return;
                }
                if (behavior.NextPatrulPoint == behavior.PatrulPoints.Length - 1)
                {
                    behavior.NextPatrulPoint = 0;
                }
                else
                {
                    behavior.NextPatrulPoint++;
                }
                ITempPointInstantiator tempPointIns = behavior.PatrulPoints[behavior.NextPatrulPoint].GetPlatformAtPoint();
                if (behavior.OttoScript.GetStepCheckerPlatform() != tempPointIns)
                {
                    behavior.ChangeSubState(new OttoPathFindWaitingState(new OttoPathFindMoveState(PatrulDelayMode, behavior),
                        behavior.OttoScript, tempPointIns.InstantiateTempPoint(behavior.PatrulPoints[behavior.NextPatrulPoint])));
                }
            }
            public static void PatrulDelayEnter(OttoBehavior behavior)
            {
                behavior.CurrentPatrulDelay = behavior.StartCoroutine(behavior.PatrulDelay());
            }
            public static void PatrulDelayExit(OttoBehavior behavior)
            {
                if (behavior.CurrentPatrulDelay != null)
                {
                    behavior.StopCoroutine(behavior.CurrentPatrulDelay);
                }
            }
            public static void RunStateEnter(OttoBehavior behavior)
            {
                behavior.OttoScript.ChangeMoveSpeed((int)Otto.EnemyMoveMode.Run);
            }
            public static void RunStateExit(OttoBehavior behavior)
            {
                behavior.OttoScript.ChangeMoveSpeed((int)Otto.EnemyMoveMode.Walk);
            }
            public static void AttackEnter(OttoBehavior behavior)
            {
                behavior.Owner.HorizontalDirection = behavior.HuntedTarget.transform.position.x - behavior.transform.position.x;
                behavior.Owner.AttackAnim();
            }
            public static void HuntingEnter(OttoBehavior behavior)
            {
                RunStateEnter(behavior);
            }
        }
        public class OttoPathFindMoveState : PathMovingState<OttoBehavior, Otto>
        {
            public OttoPathFindMoveState(IAIState<OttoBehavior, Otto> NextState, OttoBehavior behavior) : base(NextState)
            {
                if (behavior.OttoScript.GetStepCheckerPlatform() is Stairs)
                {
                    updateAction = TransitStateUpdateAction;
                }
                else
                {
                    updateAction = SimpleStateUpdateAction;
                }
                onExitAction = delegate {
                    behavior.NextPathPoint = -1;
                    behavior.CurrentPath = null;
                };
            }
            private void StateUpdateAction(OttoBehavior behavior, Action onMoveEndAction)
            {
                if (behavior.NextPathPoint < 0)
                {
                    behavior.NextPathPoint = 0;
                }
                UpdateActions.BaseMoveToTarget(behavior, behavior.CurrentPath[behavior.NextPathPoint],
                    Registry.Registry.СonstData.MovePointEndDistance, onMoveEndAction);
            }
            protected void SimpleStateUpdateAction(OttoBehavior behavior)
            {
                StateUpdateAction(behavior, delegate
                {
                    if (behavior.NextPathPoint == behavior.CurrentPath.Length - 1)
                    {
                        behavior.ChangeBehaviorState(NextState);
                    }
                    else
                    {
                        behavior.NextPathPoint++;
                        if (behavior.CurrentPath[behavior.NextPathPoint].y < behavior.CurrentPath[behavior.NextPathPoint - 1].y)
                        {
                            behavior.OttoScript.SetPassability(true);
                            updateAction = TransitStateUpdateAction;
                        }
                    }
                });
            }
            protected void TransitStateUpdateAction(OttoBehavior behavior)
            {
                StateUpdateAction(behavior, delegate
                {
                    if (behavior.NextPathPoint == behavior.CurrentPath.Length - 1)
                    {
                        behavior.OttoScript.SetPassability(false);
                        behavior.ChangeBehaviorState(NextState);
                    }
                    else
                    {
                        behavior.NextPathPoint++;
                        if (behavior.CurrentPath[behavior.NextPathPoint].y >= behavior.CurrentPath[behavior.NextPathPoint - 1].y)
                        {
                            behavior.OttoScript.SetPassability(false);
                            updateAction = SimpleStateUpdateAction;
                        }
                    }
                });
            }
            private Action<OttoBehavior> updateAction;
            public override Action<OttoBehavior> UpdateAction => updateAction;
            private readonly Action onExitAction;
            public override Action OnSubExitAction => onExitAction;
        }
        public class OttoPathFindWaitingState : PathFindWaitingState<OttoBehavior, Otto>
        {
            public OttoPathFindWaitingState(IAIState<OttoBehavior, Otto> NextState, GameCharacter start, GameCharacter end) :
                base(NextState, start, end)
            { }
            public OttoPathFindWaitingState(IAIState<OttoBehavior, Otto> NextState, GameCharacter start, ITemporalPathPoint end) :
                base(NextState, start, end)
            { }
            public override Action<OttoBehavior> UpdateAction =>
                delegate (OttoBehavior behavior)
                {
                    if (threadState.isDone)
                    {
                        if (threadState.Path != null)
                        {
                            behavior.CurrentPath = new Vector2[threadState.Path.Length - 1];
                            for (int i = 0; i < behavior.CurrentPath.Length; i++)
                            {
                                behavior.CurrentPath[i] = threadState.Path[i + 1];
                            }
                            behavior.ChangeSubState(NextState as OttoPathFindMoveState);
                        }
                        else
                        {
                            behavior.ChangeBehaviorState(PatrulDelayMode);
                        }
                    }
                };
        }
        public class OttoFallingState : FallingState<OttoBehavior, Otto>
        {
            public OttoFallingState(IAIState<OttoBehavior, Otto> NextState) : base(NextState) { }
            public override Action<OttoBehavior, HitPointSystem.DamageHitInfo> OnTakeDamage =>
                delegate (OttoBehavior behavior, HitPointSystem.DamageHitInfo hitInfo)
                {
                    if (hitInfo.Owner != null)
                    {
                        if (behavior.HuntedTarget == null)
                        {
                            NextState = HuntingMode;
                        }
                        behavior.HuntedTarget = hitInfo.Owner;
                    }
                };
            public override Action<OttoBehavior, GameCharacter> OnFindEnemy =>
                delegate (OttoBehavior behavior, GameCharacter target)
                {
                    if (behavior.HuntedTarget == null)
                    {
                        NextState = HuntingMode;
                    }
                    behavior.HuntedTarget = target;
                };
            public override Action<OttoBehavior> OnLostEnemy =>
                delegate (OttoBehavior behavior)
                {
                    behavior.HuntedTarget = null;
                    NextState = PatrulMode;
                };
            public override string Name => "FallingState";
        }

        public static readonly AIState PatrulMode = new AIState
            (UpdateActions.PatrulUpdate,
            TakeDamageEvents.OnPeacefulDamage,
            FindEnemyEvents.OnPeacefulFind,
            delegate { },
            delegate { },
            StateActions.PatrulEnter,
            delegate { },
            "PatrulMode");

        public static readonly AIState PatrulDelayMode = new AIState
            (delegate { },
            TakeDamageEvents.OnPeacefulDamage,
            FindEnemyEvents.OnPeacefulFind,
            delegate { },
            delegate { },
            StateActions.PatrulDelayEnter,
            StateActions.PatrulDelayExit,
            "PatrulDelayMode");

        public static readonly AIState HurtingMode = new AIState
            (delegate { },
            TakeDamageEvents.ReTakeDamage,
            delegate { },
            OtherMethods.SetDefaultState,
            delegate (OttoBehavior behavior) { behavior.ChangeBehaviorState(HuntingMode); },
            delegate { },
            delegate { },
            "HurtingMode");

        public static readonly AIState HuntingMode = new AIState
            (UpdateActions.HuntingUpdate,
            TakeDamageEvents.TakeDamage,
            delegate { },
            OtherMethods.SetDefaultState,
            delegate { },
            StateActions.RunStateEnter,
            StateActions.RunStateExit,
            "HuntingMode");

        public static readonly AIState IdleMode = new AIState
            (delegate { },
            TakeDamageEvents.OnPeacefulDamage,
            FindEnemyEvents.OnPeacefulFind,
            delegate { },
            delegate { },
            delegate { },
            delegate { },
            "IdleMode");

        public static readonly AIState AttackMode = new AIState
            (delegate { },
            TakeDamageEvents.TakeDamage,
            delegate { },
            delegate { },
            delegate (OttoBehavior behavior) { behavior.ChangeBehaviorState(HuntingMode); },
            StateActions.AttackEnter,
            delegate { },
            "AttackMode");

    }
}