using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Registry;

namespace GamingObjects
{
    public sealed partial class CharacterController : MonoBehaviour, ISingltone<CharacterController>
    {
        private static CharacterController Singltone=null;
        CharacterController ISingltone<CharacterController>.Singltone { get => Singltone; set => Singltone=value; }
        public static bool isPressedJump()
        {
            return Input.GetButton("Jump");
        }
        public static bool isClickedJump()
        {
            return Input.GetButtonDown("Jump");
        }
        public static bool isPressedSprint()
        {
            return Input.GetButton("Sprint");
        }
        public static bool isPressedMove()
        {
            return Input.GetButton("Horizontal");
        }
        public static bool isClickedMove()
        {
            return Input.GetButtonDown("Horizontal");
        }
        public static float GetMoveAxis()
        {
            return Input.GetAxis("Horizontal");
        }
        public static bool isPressedDown()
        {
            return Input.GetButton("Down");
        }
        public static bool isPressedUp()
        {
            return Input.GetButton("Up");
        }
        public static bool isClickedAttack1()
        {
            return Input.GetButtonDown("Attack1");
        }
        public static bool isClickedAttack2()
        {
            return Input.GetButtonDown("Attack2");
        }
        public static bool isClickedRoll()
        {
            return Input.GetButtonDown("Roll");
        }
        public static bool isPressedBlock()
        {
            return Input.GetButton("Block");
        }
        public static bool isPressedPullOver()
        {
            return Input.GetButton("PullOver");
        }
        public static bool isClickedPullOver()
        {
            return Input.GetButtonDown("PullOver");
        }
        public static IMovableObject GetMovableObject()
        {
            Collider2D coll = Physics2D.OverlapBox(new Vector2
                (Registry.Registry.MHScript.transform.position.x +
                Registry.Registry.MainHeroSettings.PullOverCheckBoxOffsetX * Registry.Registry.MHScript.HorizontalDirection,
                Registry.Registry.MHScript.transform.position.x), Registry.Registry.MainHeroSettings.PullOverCheckBoxSize, 0,
                Registry.Registry.LevelLayer);
            if (coll!=null&&coll.TryGetComponent(out IMovableObject obj))
            {
                return obj;
            }
            return null;
        }
        public static bool TryGetMovableObject(out IMovableObject obj)
        {
            obj = GetMovableObject();
            return obj == null;
        }
        IEnumerator JumpLockFallingDelay()
        {
            yield return new WaitForSeconds(Registry.Registry.MainHeroSettings.JumpingLockDelay);
            ChangeUpdateMode(ControllerMainUpdateMode.FallingMode);
            CurrentJumpLockFallingDelay = null;
            yield break;
        }
        private Coroutine CurrentJumpLockFallingDelay;
        IEnumerator JumpUnlockLandingDelay()
        {
            yield return new WaitForSeconds(Registry.Registry.MainHeroSettings.JumpLandingDelay);
            UnlockJumping();
            CurrentJumpUnlockLandingDelay = null;
            yield break;
        }
        private Coroutine CurrentJumpUnlockLandingDelay;
        private void UnlockJumping()
        {
            if (isPressedSprint())
            {
                ChangeUpdateMode(ControllerMainUpdateMode.RunningMode);
            }
            else
            {
                ChangeUpdateMode(ControllerMainUpdateMode.DefaultMode);
            }
        }
        private float CurrentFirstJumpLevel = 0;
        [SerializeField]
        private MainHero MainHeroScript;
        public ControllerUpdateMode UpdateMode { get; private set; }
        public void StopJumpLockFallingDelay()
        {
            StopCoroutine(CurrentJumpLockFallingDelay);
        }
        public void StopJumpUnlockLandingDelay()
        {
            StopCoroutine(CurrentJumpUnlockLandingDelay);
        }
        private void ChangeUpdateMode(ControllerUpdateMode mode)
        {
            UpdateMode.ExitAction();
            mode.EnterAction();
            UpdateMode = mode;
        }
        private void SetUpdate()
        {
            void SetMode(ControllerUpdateMode mode)
            {
                mode.EnterAction();
                UpdateMode = mode;
            }
            ControllerUpdateMode.controller = this;
            if (MainHeroScript.isFalling)
            {
                SetMode(ControllerMainUpdateMode.FallingMode);
            }
            else
            {
                SetMode(ControllerMainUpdateMode.DefaultMode);
            }
        }
        private void Update()
        {
            UpdateMode.UpdateAction();
        }
        private void Start()
        {
            SetUpdate();
            MainHeroScript.OnFallingEvent += delegate { UpdateMode.OnFallingAction(); };
            MainHeroScript.OnLandingEvent += delegate { UpdateMode.OnLandingAction(); };
            MainHeroScript.OnAnimationEndEvent += delegate { UpdateMode.OnAnimationEndAction(); };
            MainHeroScript.OnTakeDamageEvent += delegate { ChangeUpdateMode(ControllerMainUpdateMode.NoneControllModes.HurtMode); };
            MainHeroScript.OnSlideWallEvent += ControllerMainUpdateMode.ControllerEventActions.SlideWall;
            MainHeroScript.OnEndSlideWallEvent += ControllerMainUpdateMode.ControllerEventActions.EndSlideWall;
        }
        private void Awake()
        {
            SingltoneStatic.Awake(this, delegate { Destroy(this); }, delegate { });
        }
    }
}
