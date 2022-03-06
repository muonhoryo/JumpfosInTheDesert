using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Registry;


namespace GamingObjects
{
    public sealed partial class CharacterController
    {
        public sealed class ControllerMainUpdateMode : ControllerUpdateMode
        {
            public sealed class AttackModes : NoneControllModes
            {
                private static void AttackEnter(MainHero.MainHeroAttackType attackType)
                {
                    controller.MainHeroScript.SetGroundAttack(attackType);
                }
                private AttackModes(Action enterAction) : base(enterAction) { }
                protected override ControllerUpdateMode ModeSource => OnGroungAttackMode;

                public static readonly AttackModes HighPunchMode =
                    new AttackModes(delegate { AttackEnter(MainHero.MainHeroAttackType.HighPunch); });

                public static readonly AttackModes KickMode =
                    new AttackModes(delegate { AttackEnter(MainHero.MainHeroAttackType.SimpleKick); });

                public static readonly AttackModes MultiPunchMode =
                    new AttackModes(delegate { AttackEnter(MainHero.MainHeroAttackType.MultiPunch); });

                public static readonly AttackModes MultiKickMode =
                    new AttackModes(delegate { AttackEnter(MainHero.MainHeroAttackType.MultiKick); });

                public static readonly AttackModes RollMode =
                    new AttackModes(controller.MainHeroScript.Roll);
            }
            public class NoneControllModes : ControllerClosedUpdateMode
            {
                protected NoneControllModes(Action enterAction)
                {
                    this.enterAction = enterAction;
                }
                private readonly Action enterAction;
                protected override ControllerUpdateMode ModeSource =>NoneControllMode;
                public override Action EnterAction => enterAction;

                public static readonly NoneControllModes HurtMode = new NoneControllModes
                    (delegate { controller.MainHeroScript.TakeDamageAnim(); });
            }
            public sealed class FallAttackModes : NoneControllModes
            {
                private static void AttackEnter(MainHero.MainHeroAttackType attackType)
                {
                    controller.MainHeroScript.SetFallingAttack(attackType);
                }
                private FallAttackModes(Action enterAction) : base(enterAction) { }
                protected override ControllerUpdateMode ModeSource => OnFallingAttackMode;

                public static readonly FallAttackModes LowPunchUpdate =
                    new FallAttackModes(delegate { AttackEnter(MainHero.MainHeroAttackType.LowPunch); });
            }
            public sealed class PullOverMode : ControllerClosedUpdateMode
            {
                public PullOverMode(IMovableObject PullOveredObject)
                {
                    this.PullOveredObject = PullOveredObject;
                }
                private readonly IMovableObject PullOveredObject;
                private void Update()
                {
                    if (isPressedPullOver()&&GetMovableObject() == PullOveredObject)
                    {
                        int direction = isPressedMove() ? Math.Sign(GetMoveAxis()) : 0;
                        controller.MainHeroScript.PullOverMove(direction);
                        PullOveredObject.Move(controller.MainHeroScript.MoveSpeed, direction);
                    }
                    else
                    {
                        ControllerEventActions.AnimationEndOnAttack();
                    }
                }
                public override Action UpdateAction => Update;
                protected override ControllerUpdateMode ModeSource => PlatformPullOverMode;
            }

            public static class ControllerEventActions
            {
                public static void FallingOnDefaultUpdate()
                {
                    controller.ChangeUpdateMode(StartFallingMode);
                }
                public static void FallingOnNoneJumpingUpdate()
                {
                    FallingOnOtherUpdate();
                    controller.StopJumpUnlockLandingDelay();
                }
                public static void FallingOnOtherUpdate()
                {
                    controller.ChangeUpdateMode(FallingMode);
                }
                public static void LandingOnStartFallingUpdate()
                {
                    LandingOnFallingUpdate();
                    controller.StopJumpLockFallingDelay();
                }
                public static void LandingOnFallingUpdate()
                {
                    controller.ChangeUpdateMode(NoneJumpingMode);
                }
                public static void AnimationEndOnAttack()
                {
                    controller.ChangeUpdateMode(DefaultMode);
                }
                public static void LandingOnFallKick()
                {
                    LandingOnFallingUpdate();
                    controller.MainHeroScript.SetGravityScale();
                }
                public static void LandingOnWallSlide()
                {
                    LandingOnFallKick();
                    controller.MainHeroScript.ChangeWallSlide(false);
                }
                public static void SlideWall(float wallDirection)
                {
                    if (controller.MainHeroScript.isFalling)
                    {
                        controller.MainHeroScript.HorizontalDirection = wallDirection;
                        controller.ChangeUpdateMode(WallSlideMode);
                    }
                }
                public static void EndSlideWall()
                {
                    if (controller.MainHeroScript.isFalling)
                    {
                        controller.ChangeUpdateMode(FallingMode);
                    }
                }
            }
            private static class ControllerUpdateActions
            {
                private static void OnGroundUpdate(Action elseAction)
                {
                    if (isPressedDown())
                    {
                        controller.ChangeUpdateMode(SittingMode);
                    }
                    else if (isPressedBlock())
                    {
                        controller.ChangeUpdateMode(BlockMode);
                    }
                    else if (isClickedAttack1())
                    {
                        controller.ChangeUpdateMode(AttackModes.KickMode);
                    }
                    else if (isClickedAttack2())
                    {
                        controller.ChangeUpdateMode(AttackModes.MultiKickMode);
                    }
                    else if (isClickedRoll())
                    {
                        if (!isPressedMove())
                        {
                            controller.MainHeroScript.HorizontalDirection *= -1;
                        }
                        controller.ChangeUpdateMode(AttackModes.RollMode);
                    }
                    else if (isPressedUp() && isClickedAttack1())
                    {
                        controller.ChangeUpdateMode(AttackModes.HighPunchMode);
                    }
                    else
                    {
                        elseAction();
                        BaseMove();
                    }
                }
                private static void BaseMove()
                {
                    controller.MainHeroScript.AnimatedMove(isPressedMove() ? Math.Sign(GetMoveAxis()) : 0);
                }
                private static void BaseFall()
                {
                    if (isPressedDown())
                    {
                        if (isClickedAttack1())
                        {
                            controller.ChangeUpdateMode(FallAttackModes.LowPunchUpdate);
                        }
                        else if (isClickedAttack2())
                        {
                            controller.ChangeUpdateMode(FallKickMode);
                        }
                    }
                    bool isFallUp = controller.MainHeroScript.VerticalVelocity > 0;
                    if (isFallUp != controller.MainHeroScript.isFallUp)
                    {
                        controller.MainHeroScript.FallingStatusChange(isFallUp ? 2 : 1);
                    }
                }
                public static void NoneJumpingUpdate()
                {
                    OnGroundUpdate(delegate
                    {
                        if (isPressedSprint())
                        {
                            controller.ChangeUpdateMode(NoneJumpingRunMode);
                        }
                    });
                }
                public static void NoneJumpinRunUpdate()
                {
                    OnGroundUpdate(delegate
                    {
                        if (!isPressedSprint())
                        {
                            controller.ChangeUpdateMode(NoneJumpingMode);
                        }
                    });
                }
                public static void DefaultUpdate()
                {
                    OnGroundUpdate(delegate
                    {
                        if (isPressedJump())
                        {
                            controller.ChangeUpdateMode(StartJumpingMode);
                        }
                        else if (isPressedSprint())
                        {
                            controller.ChangeUpdateMode(RunningMode);
                        }
                        else if(isClickedPullOver()&&TryGetMovableObject(out IMovableObject obj))
                        {
                            controller.ChangeUpdateMode(new PullOverMode(obj));
                        }
                    });
                }
                public static void RunningUpdate()
                {
                    OnGroundUpdate(delegate
                    {
                        if (isPressedJump())
                        {
                            controller.ChangeUpdateMode(StartJumpingMode);
                        }
                        else if (!isPressedSprint())
                        {
                            controller.ChangeUpdateMode(DefaultMode);
                        }
                    });
                }
                public static void StartFallingUpdate()
                {
                    if (isPressedJump())
                    {
                        controller.ChangeUpdateMode(StartJumpingMode);
                    }
                    else
                    {
                        BaseFall();
                    }
                    BaseMove();
                }
                public static void FallingUpdate()
                {
                    BaseFall();
                    BaseMove();
                }
                public static void StartJumpingUpdate()
                {
                    if (isPressedJump() &&
                        controller.CurrentFirstJumpLevel < Registry.Registry.MainHeroSettings.MaxFirstJumpLevel)
                    {
                        BaseFall();
                        controller.CurrentFirstJumpLevel += Time.deltaTime;
                        controller.MainHeroScript.FirstJump(Registry.Registry.MainHeroSettings.JumpForce);
                    }
                    else
                    {
                        if (!controller.MainHeroScript.isFalling)
                        {
                            ControllerEventActions.LandingOnFallingUpdate();
                        }
                        else
                        {
                            BaseFall();
                            controller.ChangeUpdateMode(JumpingMode);
                        }
                    }
                    BaseMove();
                }
                public static void JumpingUpdate()
                {
                    if (isClickedJump())
                    {
                        controller.MainHeroScript.Jump(Registry.Registry.MainHeroSettings.SecondJumpForce);
                        controller.ChangeUpdateMode(FallingMode);
                    }
                    else
                    {
                        BaseFall();
                    }
                    BaseMove();
                }
                public static void SittingUpdate()
                {
                    if (!isPressedDown())
                    {
                        controller.ChangeUpdateMode(DefaultMode);
                    }
                    else if (isPressedJump() && !controller.MainHeroScript.DownAction(Registry.Registry.MainHeroSettings.DownActionDelay))
                    {
                        controller.ChangeUpdateMode(FallingMode);
                    }
                    else if (isClickedAttack1())
                    {
                        controller.ChangeUpdateMode(AttackModes.MultiPunchMode);
                    }
                }
                public static void BlockUpdate()
                {
                    if (!isPressedBlock())
                    {
                        controller.ChangeUpdateMode(DefaultMode);
                    }
                }
                public static void AttackUpdate()
                {
                    if (isPressedJump())
                    {
                        controller.ChangeUpdateMode(StartJumpingMode);
                    }
                }
                public static void WallSlideUpdate()
                {
                    if (isClickedMove() && Math.Sign(GetMoveAxis()) == controller.MainHeroScript.HorizontalDirection)
                    {
                        controller.ChangeUpdateMode(FallingMode);
                    }
                    if (isClickedJump())
                    {
                        controller.MainHeroScript.WallJump(Registry.Registry.MainHeroSettings.WallJumpForce,
                            Registry.Registry.MainHeroSettings.WallJumpAngle);
                        controller.ChangeUpdateMode(FallingMode);
                    }
                }
            }
            private static class ControllerStateActions
            {
                public static void NoneJumpingEnterAction()
                {
                    controller.CurrentJumpUnlockLandingDelay = controller.StartCoroutine(controller.JumpUnlockLandingDelay());
                }
                public static void NoneJumpingExitAction()
                {
                    if (controller.CurrentJumpUnlockLandingDelay != null)
                    {
                        controller.StopCoroutine(controller.CurrentJumpUnlockLandingDelay);
                    }
                }
                public static void StartFallingEnterAction()
                {
                    if (controller.CurrentJumpLockFallingDelay != null)
                    {
                        controller.StopCoroutine(controller.CurrentJumpLockFallingDelay);
                    }
                    controller.CurrentJumpLockFallingDelay = controller.StartCoroutine(controller.JumpLockFallingDelay());
                }
                public static void StartJumpingEnterAction()
                {
                    void landingOnStartJumping()
                    {
                        controller.CurrentFirstJumpLevel = 0;
                        controller.MainHeroScript.OnLandingEvent -= landingOnStartJumping;
                    }
                    controller.MainHeroScript.OnLandingEvent += landingOnStartJumping;
                    controller.CurrentFirstJumpLevel += Time.deltaTime;
                    controller.MainHeroScript.FirstJump(Registry.Registry.MainHeroSettings.JumpForce);
                    FallingChangeSpeedEnterAction();
                }
                public static void FallingChangeSpeedEnterAction()
                {
                    controller.MainHeroScript.ChangeMoveSpeed((int)MainHero.MainHeroMoveMode.Fall);
                }
                public static void FallingChangeSpeedExitAction()
                {
                    controller.MainHeroScript.ChangeMoveSpeed((int)MainHero.MainHeroMoveMode.Walk);
                }
                public static void RunningEnterAction()
                {
                    controller.MainHeroScript.ChangeMoveSpeed((int)MainHero.MainHeroMoveMode.Run);
                }
                public static void RunningExitAction()
                {
                    controller.MainHeroScript.ChangeMoveSpeed((int)MainHero.MainHeroMoveMode.Walk);
                }
                public static void AttackExitAction()
                {
                    controller.MainHeroScript.ResetAttack();
                }
                public static void FallKickEnterAction()
                {
                    controller.MainHeroScript.SetFallingAttack(MainHero.MainHeroAttackType.FallKick);
                }
                public static void WallSlideEnterAction()
                {
                    FallingChangeSpeedEnterAction();
                    controller.MainHeroScript.SetGravityScale(Registry.Registry.MainHeroSettings.OnWallSlideGravity);
                    controller.MainHeroScript.ChangeWallSlide(true);
                }
                public static void WallSlideExitAction()
                {
                    controller.MainHeroScript.ChangeWallSlide(false);
                    FallingChangeSpeedExitAction();
                    controller.MainHeroScript.SetGravityScale();
                }
                public static void PullOverEnterAction()
                {
                    controller.MainHeroScript.ChangeMoveSpeed((int)MainHero.MainHeroMoveMode.PullOver);
                }
                public static void PullOverExitAction()
                {
                    controller.MainHeroScript.ChangeMoveSpeed((int)MainHero.MainHeroMoveMode.Walk);
                    controller.MainHeroScript.SetAnimationSpeed();
                }
            }
            private ControllerMainUpdateMode() { }
            private ControllerMainUpdateMode(Action UpdateAction, Action OnFallingAction, Action OnLandingAction,
                Action OnAnimationEndAction, Action EnterAction, Action ExitAction, string name)
            {
                modeName = name;
                updateAction = UpdateAction;
                onFallingAction = OnFallingAction;
                onLandingAction = OnLandingAction;
                onAnimationEndAction = OnAnimationEndAction;
                enterAction = EnterAction;
                exitAction = ExitAction;
            }
            private readonly string modeName;
            private readonly Action updateAction;
            private readonly Action onFallingAction;
            private readonly Action onLandingAction;
            private readonly Action onAnimationEndAction;
            private readonly Action enterAction;
            private readonly Action exitAction;
            public override string Name => modeName;
            public override Action OnAnimationEndAction { get => delegate { onAnimationEndAction(); }; }
            public override Action UpdateAction { get => delegate { updateAction(); }; }
            public override Action OnFallingAction { get => delegate { onFallingAction(); }; }
            public override Action OnLandingAction { get => delegate { onLandingAction(); }; }
            public override Action EnterAction => enterAction;
            public override Action ExitAction => exitAction;

            private static readonly ControllerMainUpdateMode PlatformPullOverMode = new ControllerMainUpdateMode
                (UpdateAction: delegate { },
                 OnFallingAction: ControllerEventActions.FallingOnDefaultUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: delegate { },
                 EnterAction: ControllerStateActions.PullOverEnterAction,
                 ExitAction: ControllerStateActions.PullOverExitAction,
                 name: "PullOverMode");

            private static readonly ControllerMainUpdateMode NoneControllMode = new ControllerMainUpdateMode
                (UpdateAction: delegate { },
                 OnFallingAction: ControllerEventActions.FallingOnOtherUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: ControllerEventActions.AnimationEndOnAttack,
                 EnterAction: delegate { },
                 ExitAction: ControllerStateActions.AttackExitAction,
                 name: "NoneControllMode");

            private static readonly ControllerMainUpdateMode OnGroungAttackMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.AttackUpdate,
                 OnFallingAction: ControllerEventActions.FallingOnOtherUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: ControllerEventActions.AnimationEndOnAttack,
                 EnterAction: delegate { },
                 ExitAction: ControllerStateActions.AttackExitAction,
                 name: "AttackMode");

            private static readonly ControllerMainUpdateMode OnFallingAttackMode = new ControllerMainUpdateMode
                (UpdateAction: delegate { },
                 OnFallingAction: delegate { },
                 OnLandingAction: ControllerEventActions.LandingOnFallingUpdate,
                 OnAnimationEndAction: ControllerEventActions.AnimationEndOnAttack,
                 EnterAction: delegate { },
                 ExitAction: ControllerStateActions.FallingChangeSpeedExitAction,
                 name: "FallAttackMode");

            public static readonly ControllerMainUpdateMode DefaultMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.DefaultUpdate,
                 OnFallingAction: ControllerEventActions.FallingOnDefaultUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: delegate { },
                 EnterAction: delegate { },
                 ExitAction: delegate { },
                 name: "DefaultMode");

            public static readonly ControllerMainUpdateMode RunningMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.RunningUpdate,
                 OnFallingAction: ControllerEventActions.FallingOnDefaultUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: delegate { },
                 EnterAction: ControllerStateActions.RunningEnterAction,
                 ExitAction: ControllerStateActions.RunningExitAction,
                 name: "RunningMode");

            public static readonly ControllerMainUpdateMode NoneJumpingMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.NoneJumpingUpdate,
                 OnFallingAction: ControllerEventActions.FallingOnNoneJumpingUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: delegate { },
                 EnterAction: ControllerStateActions.NoneJumpingEnterAction,
                 ExitAction: ControllerStateActions.NoneJumpingExitAction,
                 name: "NoneJumpingMode");

            public static readonly ControllerMainUpdateMode NoneJumpingRunMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.NoneJumpinRunUpdate,
                 OnFallingAction: ControllerEventActions.FallingOnNoneJumpingUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: delegate { },
                 EnterAction: delegate { ControllerStateActions.NoneJumpingEnterAction(); ControllerStateActions.RunningEnterAction(); },
                 ExitAction: ControllerStateActions.RunningExitAction,
                 name: "NoneJumpingRunMode");

            public static readonly ControllerMainUpdateMode StartFallingMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.StartFallingUpdate,
                 OnFallingAction: delegate { },
                 OnLandingAction: ControllerEventActions.LandingOnStartFallingUpdate,
                 OnAnimationEndAction: delegate { },
                 EnterAction: delegate { ControllerStateActions.StartFallingEnterAction(); },
                 ExitAction: ControllerStateActions.FallingChangeSpeedExitAction,
                 name: "StartFallingMode");

            public static readonly ControllerMainUpdateMode FallingMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.FallingUpdate,
                 OnFallingAction: delegate { },
                 OnLandingAction: ControllerEventActions.LandingOnFallingUpdate,
                 OnAnimationEndAction: delegate { },
                 EnterAction: ControllerStateActions.FallingChangeSpeedEnterAction,
                 ExitAction: ControllerStateActions.FallingChangeSpeedExitAction,
                 name: "FallingMode");

            public static readonly ControllerMainUpdateMode StartJumpingMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.StartJumpingUpdate,
                 OnFallingAction: delegate { },
                 OnLandingAction: ControllerEventActions.LandingOnFallingUpdate,
                 OnAnimationEndAction: delegate { },
                 EnterAction: ControllerStateActions.StartJumpingEnterAction,
                 ExitAction: ControllerStateActions.FallingChangeSpeedExitAction,
                 name: "StartJumpingMode");

            public static readonly ControllerMainUpdateMode JumpingMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.JumpingUpdate,
                 OnFallingAction: delegate { },
                 OnLandingAction: ControllerEventActions.LandingOnFallingUpdate,
                 OnAnimationEndAction: delegate { },
                 EnterAction: ControllerStateActions.FallingChangeSpeedEnterAction,
                 ExitAction: ControllerStateActions.FallingChangeSpeedExitAction,
                 name: "JumpingMode");

            public static readonly ControllerMainUpdateMode SittingMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.SittingUpdate,
                 OnFallingAction: ControllerEventActions.FallingOnOtherUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: delegate { },
                 EnterAction: delegate { controller.MainHeroScript.SitDown(true); },
                 ExitAction: delegate { controller.MainHeroScript.SitDown(false); },
                 name: "SittingMode");

            public static readonly ControllerMainUpdateMode BlockMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.BlockUpdate,
                 OnFallingAction: ControllerEventActions.FallingOnOtherUpdate,
                 OnLandingAction: delegate { },
                 OnAnimationEndAction: delegate { },
                 EnterAction: delegate { controller.MainHeroScript.Block(true); },
                 ExitAction: delegate { controller.MainHeroScript.Block(false); },
                 name: "BlockUpdate");

            public static readonly ControllerMainUpdateMode FallKickMode = new ControllerMainUpdateMode
                (UpdateAction: delegate { },
                 OnFallingAction: delegate { },
                 OnLandingAction: ControllerEventActions.LandingOnFallingUpdate,
                 OnAnimationEndAction: ControllerEventActions.AnimationEndOnAttack,
                 EnterAction: ControllerStateActions.FallKickEnterAction,
                 ExitAction: ControllerStateActions.FallingChangeSpeedExitAction,
                 name: "FallKickMode");

            public static readonly ControllerMainUpdateMode WallSlideMode = new ControllerMainUpdateMode
                (UpdateAction: ControllerUpdateActions.WallSlideUpdate,
                 OnFallingAction: delegate { },
                 OnLandingAction: ControllerEventActions.LandingOnWallSlide,
                 OnAnimationEndAction: delegate { },
                 EnterAction: ControllerStateActions.WallSlideEnterAction,
                 ExitAction: ControllerStateActions.WallSlideExitAction,
                 name: "WallSlideMode");
        }
    }
}