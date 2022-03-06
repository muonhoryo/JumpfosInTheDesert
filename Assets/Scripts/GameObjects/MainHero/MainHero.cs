using Registry;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class MainHero : GameCharacter, ISpeedModeCharacter<MainHero.MainHeroMoveMode>, ISingltone<MainHero>
    {
        //Events
        public void OnSlideWall(float wallDirection)
        {
            OnSlideWallEvent(wallDirection);
        }
        public event Action<float> OnSlideWallEvent=delegate{};
        public void OnEndSlideWall()
        {
            OnEndSlideWallEvent();
        }
        public event Action OnEndSlideWallEvent = delegate { };
        //Assingments
        [SerializeField]
        private MainHeroHPSystem HeroHPSystem;
        public override HitPointSystem HitPointSystem => HeroHPSystem;
        public override CharacterSettings ConstData => Registry.Registry.MainHeroSettings;
        //Moving
        private float moveSpeed;
        protected override float MoveSpeed => moveSpeed;
        float ISpeedModeCharacter<MainHeroMoveMode>.MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public static readonly Func<float>[] moveSpeedConsts = new Func<float>[4]
        {
            ()=>Registry.Registry.MainHeroSettings.WalkSpeed,
            ()=>Registry.Registry.MainHeroSettings.RunSpeed,
            ()=>Registry.Registry.MainHeroSettings.OnFallingMoveSpeed,
            ()=>Registry.Registry.MainHeroSettings.PullOverSpeed
        };
        Func<float>[] ISpeedModeCharacter<MainHeroMoveMode>.MoveSpeedConsts => moveSpeedConsts;
        public enum MainHeroMoveMode
        {
            Walk,
            Run,
            Fall,
            PullOver
        }
        public void ChangeMoveSpeed(int moveMode)
        {
            Animator.SetBool("Run", moveMode == (int)MainHeroMoveMode.Run);
            SpeedModeCharacter.ChangeMoveSpeed(this,moveMode);
        }
        public void PullOverMove(int direction)
        {
            if (direction == 0)
            {
                if (isMove)
                {
                    OnStopMove();
                    isMove = false;
                    SetAnimationSpeed(0);
                }
            }
            else
            {
                Move(MoveSpeed,direction);
                Animator.SetInteger("PullOver", (int)(direction * HorizontalDirection));
                if (!isMove)
                {
                    OnStartMove();
                    isMove = true;
                    SetAnimationSpeed();
                }
            }
        }
        //Attack
        public enum MainHeroAttackType
        {
            SimpleKick=1,
            HighPunch,
            LowPunch,
            FallKick,
            MultiPunch,
            MultiKick
        }
        public void SetGroundAttack(MainHeroAttackType attackType)
        {
            Animator.SetInteger("AttackType", (int)attackType);
            Animator.SetTrigger("Attack");
        }
        public void SetFallingAttack(MainHeroAttackType attackType)
        {
            SetGroundAttack(attackType);
            SetJump(ResetJump);
        }
        public void ResetAttack()
        {
            Animator.SetInteger("AttackType", 0);
        }
        //Vertical moving
            //Jump
        private bool isJump = false;
        private void SetJump(Action resetJumpAction)
        {
            Animator.SetBool("isJump", true);
            isJump = true;
            OnLandingEvent += resetJumpAction;
        }
        public void FirstJump(float jumpForce)
        {
            if (!isJump)
            {
                Animator.SetTrigger("Jump");
                SetJump(ResetJump);
            }
            RGbody.AddForce(Vector2.up * jumpForce / Time.deltaTime * Registry.Registry.MainHeroSettings.JumpForceModifier, ForceMode2D.Force);
        }
        private void ResetJump()
        {
            isJump = false;
            Animator.SetBool("isJump", false);
            OnLandingEvent -= ResetJump;
        }
        public void Jump(float jumpForce)
        {
            Animator.SetTrigger("Jump");
            if (!isJump)
            {
                SetJump(ResetJump);
            }
            RGbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        public void WallJump(float jumpForce,Vector2 jumpDirection)
        {
            SetJump(ResetJump);
            Animator.SetTrigger("WallSlideJump");
            RGbody.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
        public void WallJump(float jumpForce,float jumpAnlge)
        {
            WallJump(jumpForce, jumpAnlge.Direction());
        }
            //Fall
        public bool isFallUp = false;
        public override void FallingStatusChange(int status)
        {
            base.FallingStatusChange(status);
            isFallUp = RGbody.velocity.y > 0;
        }
        protected override void OnFallingAction()
        {
            FallingStatusChange(RGbody.velocity.y > 0 ? 2 : 1);
        }
            //DownAction
        private IEnumerator DownActionDelay(float delay)
        {
            if (!IgnoringHalfMovebleLayers)
            {
                IgnoringHalfMovebleLayers = true;
            }
            yield return new WaitForSeconds(delay);
            IgnoringHalfMovebleLayers = false;
            CurrentDownActionDelay = null;
            yield break;
        }
        private Coroutine CurrentDownActionDelay;
        /// <summary>
        /// Return false if jump off
        /// </summary>
        /// <param name="isAction"></param>
        /// <returns></returns>
        public bool DownAction(float delay)
        {
            if (CurrentDownActionDelay != null)
            {
                StopCoroutine(CurrentDownActionDelay);
            }
            CurrentDownActionDelay = StartCoroutine(DownActionDelay(delay));
            return !isFalling;
        }
            //WallSlide
        public void ChangeWallSlide(bool isSlide)
        {
            Animator.SetBool("isWallSlide", isSlide);
        }
            //Other
        public void SitDown(bool isAction)
        {
            Animator.SetBool("Sit", isAction);
        }
        public void Block(bool isAction)
        {
            Animator.SetBool("Block", isAction);
        }
        public void Roll()
        {
            Animator.SetTrigger("Roll");
        }
        private void Start()
        {
            moveSpeed = ConstData.WalkSpeed;
        }
        MainHero ISingltone<MainHero>.Singltone { get => Registry.Registry.MHScript; set => Registry.Registry.MHScript=value; }
        protected override void Awake()
        {
            SingltoneStatic.Awake(this, delegate { Destroy(this); },
                delegate
                {
                    base.Awake();
                    OnTakeDamageEvent += delegate (HitPointSystem.DamageHitInfo hitInfo)
                    { RGbody.AddForce(hitInfo.HitDirectionVect2 * -hitInfo.HitForce, ForceMode2D.Impulse); };
                });
        }
    }
}
