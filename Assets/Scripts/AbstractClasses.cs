using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using PathFinding;

namespace GamingObjects
{
    public abstract class GameCharacter : MonoBehaviour, IMovableObject
    {
        //Assigned in Inspector
        [SerializeField]
        protected MeleeHitter MeleeHitter;
        [SerializeField]
        protected StepChecker StepChecker;
        [SerializeField]
        protected SpriteRenderer SpriteRenderer;
        [SerializeField]
        protected Rigidbody2D RGbody;
        [SerializeField]
        protected Animator Animator;
        //Assigned in inhertor
        public abstract CharacterSettings ConstData { get; }
        public abstract HitPointSystem HitPointSystem { get; }
        //Events
        public event Action OnChangePlatformEvent
        {
            add { StepChecker.OnChangePlatformEvent += value; }
            remove { StepChecker.OnChangePlatformEvent -= value; }
        }
        public event Action<HitPointSystem.DamageHitInfo> OnTakeDamageEvent
        {
            add { HitPointSystem.OnTakeDamageEvent += value; }
            remove { HitPointSystem.OnTakeDamageEvent -= value; }
        }
        public event Action OnFallingEvent
        {
            add { StepChecker.OnFallingEvent += value; }
            remove { StepChecker.OnFallingEvent -= value; }
        }
        public event Action OnLandingEvent
        {
            add { StepChecker.OnLandingEvent += value; }
            remove { StepChecker.OnLandingEvent -= value; }
        }
        public event Action OnAnimationEndEvent = delegate { };
        public void OnAnimationEnd()
        {
            OnAnimationEndEvent();
        }
        public void OnStopMove()
        {
            OnStopMoveEvent();
        }
        public void OnStartMove()
        {
            OnStartMoveEvent();
        }
        public event Action OnPassabilityChangeEvent = delegate { };
        public event Action OnDirectionChangeEvent = delegate { };
        public event Action OnStartMoveEvent = delegate { };
        public event Action OnStopMoveEvent = delegate { };
        //Platform passability
        private bool ignoringHalfMovebleLayers = false;
        public bool IgnoringHalfMovebleLayers
        {
            get => ignoringHalfMovebleLayers;
            protected set
            {
                bool oldValue = ignoringHalfMovebleLayers;
                ignoringHalfMovebleLayers = value;
                if (oldValue != value)
                {
                    gameObject.layer = value ? ConstData.IgnoringHalfPassLayer : ConstData.DefaultLayer;
                    OnPassabilityChangeEvent();
                }
            }
        }
        //Horizontal direction view of character
        private void SetHorizontalDirection(float direction,float DefaultMeleeHitterX)
        {
            bool isBackDirection = direction < 0;
            SpriteRenderer.flipX = isBackDirection;
            MeleeHitter.HitDirection = !isBackDirection ? 90 : 270;
            MeleeHitter.transform.localPosition = new Vector2(isBackDirection ? -DefaultMeleeHitterX : DefaultMeleeHitterX,
                MeleeHitter.transform.localPosition.y);
        }
        private void SetHorizontalDirection(float direction)
        {
            SetHorizontalDirection(direction, MeleeHitter.DefaultX);
        }
        private float horizontalDirection;
        public float HorizontalDirection 
        {
            get => horizontalDirection;
            set
            {
                value = Mathf.Sign(value);
                if (value != horizontalDirection)
                {
                    OnDirectionChangeEvent();
                    SetHorizontalDirection(value);
                }
                horizontalDirection = value;
            } 
        }
        //Falling
        public float VerticalVelocity { get => RGbody.velocity.y; }
        public bool isSwimming { get; protected set; } = false;
        public bool isFalling { get; protected set; } = true;
        protected virtual void OnFallingAction()
        {
            FallingStatusChange(1);
        }
        protected virtual void OnLandingAction()
        {
            FallingStatusChange(0);
        }
        public virtual void FallingStatusChange(int status)
        {
            Animator.SetInteger("Fall", status);
            isFalling = status != 0;
        }
        //Moving
        public float DefaultGravityScale { get; protected set; }
        public Vector2 MoveForceDirection { get; protected set; } = Vector2.right;
        public virtual float MoveSpeed { get => ConstData.WalkSpeed; }
        /// <summary>
        /// 1-if falling,0-if not
        /// </summary>
        /// <param name="status"></param>
        public bool isMove { get; protected set; } = false;
        public virtual void AnimatedMove(int direction)
        {
            if (direction==0)
            {
                if (isMove)
                {
                    OnStopMoveEvent();
                    isMove = false;
                    Animator.SetBool("Move", false);
                }
            }
            else
            {
                HorizontalDirection = direction;
                Move(MoveSpeed);
                if (!isMove)
                {
                    OnStartMoveEvent();
                    Animator.SetBool("Move", true);
                    isMove = true;
                }
            }
        }
        public virtual void Move(float moveForce)
        {
            Move(moveForce, (int)HorizontalDirection);
        }
        public virtual void Move(float moveForce,int direction)
        {
            RGbody.MoveObject(MoveForceDirection * direction, moveForce);
        }
        public virtual void SetMoveForceDirection(Vector2 forceDirection)
        {
            MoveForceDirection = forceDirection;
        }
        public void SetMoveForceDirection()
        {
            SetMoveForceDirection(Vector2.right);
        }
        public virtual void SetGravityScale(float gravityScale)
        {
            RGbody.gravityScale = gravityScale;
        }
        public void SetGravityScale()
        {
            SetGravityScale(DefaultGravityScale);
        }
        //Fighting
        public virtual void MeleeAttack()
        {
            MeleeHitter.DamageAllSystems();
        }
        public virtual void TakeDamageAnim()
        {
            Animator.SetTrigger("TakeDMG");
        }
        //Other
        public ITempPointInstantiator GetStepCheckerPlatform()
        {
            return StepChecker.CurrentPlatform;
        }
        public ITemporalPathPoint GetNewPathPoint()
        {
            if (StepChecker.CurrentPlatform == null)
            {
                return ((Vector2)transform.position).GetPlatformAtPoint().InstantiateTempPoint(transform.position);
            }
            else
            {
                return StepChecker.CurrentPlatform.InstantiateTempPoint(transform.position);
            }
        }
        protected virtual void Awake()
        {
            SetHorizontalDirection(!SpriteRenderer.flipX?1:-1,
                !SpriteRenderer.flipX?MeleeHitter.transform.localPosition.x:-MeleeHitter.transform.localPosition.x);
            horizontalDirection = !SpriteRenderer.flipX ? 1 : -1;
            DefaultGravityScale = RGbody.gravityScale;
            StepChecker.OnFallingEvent += OnFallingAction;
            StepChecker.OnLandingEvent += OnLandingAction;
        }
        public void SetAnimationSpeed(float speed=1)
        {
            Animator.speed = speed;
        }
    }
    public abstract class EnemyCharacter : GameCharacter
    {
        public void AttackAnim()
        {
            Animator.SetTrigger("Attack");
        }
        public virtual void TakeDamage(HitPointSystem.DamageHitInfo hitInfo)
        {
            TakeDamageAnim();
            RGbody.AddForce(hitInfo.HitDirectionVect2 * -hitInfo.HitForce, ForceMode2D.Impulse);
        }
    }
    public abstract class RunningEnemy : EnemyCharacter, ISpeedModeCharacter<RunningEnemy.EnemyMoveMode>
    {
        protected abstract Func<float>[] MoveSpeedConsts { get; }
        protected float moveSpeed;
        float ISpeedModeCharacter<EnemyMoveMode>.MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        Func<float>[] ISpeedModeCharacter<EnemyMoveMode>.MoveSpeedConsts { get => MoveSpeedConsts; }
        public override float MoveSpeed{ get => moveSpeed; }
        public enum EnemyMoveMode
        {
            Walk,
            Run
        }
        public void ChangeMoveSpeed(int moveMode)
        {
            Animator.SetBool("Run", moveMode == (int)EnemyMoveMode.Run);
            SpeedModeCharacter.ChangeMoveSpeed(this,moveMode);
        }
    }
    public abstract class HitPointSystem : MonoBehaviour
    {
        public abstract GameCharacter Owner { get; }
        public struct DamageHitInfo
        {
            public DamageHitInfo(int HitDamage, float HitDirection, GameCharacter Owner, float HitForce = 0)
            {
                this.HitDamage = HitDamage;
                this.HitForce = HitForce;
                this.HitDirection = HitDirection;
                this.Owner = Owner;
            }
            public DamageHitInfo(int HitDamage, Vector2 HitDirection, GameCharacter Owner, float HitForce = 0)
            {
                this.HitDamage = HitDamage;
                this.HitDirection = Vector2.Angle(Vector2.zero, HitDirection);
                this.HitForce = HitForce;
                this.Owner = Owner;
            }
            public int HitDamage;
            public float HitDirection;
            public float HitForce;
            public GameCharacter Owner;
            public Vector2 HitDirectionVect2 { get => HitDirection.Direction(); }
        }
        [SerializeField]
        protected int hitPoints;
        [SerializeField]
        private bool IsImmortal = false;
        public event Action OnChangeMortalityEvent = delegate { };
        public bool isImmortal
        {
            get => IsImmortal;
            protected set
            {
                bool oldMortality = IsImmortal;
                IsImmortal = value;
                if (oldMortality != value)
                {
                    OnChangeMortalityEvent();
                }
            }
        }
        public void SetMortality(bool isImmortal)
        {
            this.isImmortal = isImmortal;
        }
        public virtual int HitPoints { get=>hitPoints; protected set=> hitPoints=value; }
        public event Action<DamageHitInfo> OnTakeDamageEvent = delegate { };
        public event Action OnDeathEvent = delegate { };
        protected void OnTakeDamage(DamageHitInfo hitInfo)
        {
            OnTakeDamageEvent(hitInfo);
        }
        public virtual void TakeDamage(DamageHitInfo hitInfo)
        {
            if (!isImmortal)
            {
                HitPoints -= hitInfo.HitDamage;
                OnTakeDamage(hitInfo);
                if (HitPoints <= 0)
                {
                    Death();
                }
            }
            else
            {
                OnTakeDamage(hitInfo);
            }
        }
        protected void OnDeath()
        {
            OnDeathEvent();
        }
        public virtual void Death()
        {
            Destroy(gameObject);
            OnDeath();
        }
    }
    public abstract class ControllerUpdateMode
    {
        protected ControllerUpdateMode() { }
        public static CharacterController controller;
        public abstract string Name { get; }
        public abstract Action OnAnimationEndAction { get; }
        public abstract Action UpdateAction { get; }
        public abstract Action OnFallingAction { get; }
        public abstract Action OnLandingAction { get; }
        public abstract Action EnterAction { get; }
        public abstract Action ExitAction { get; }
    }
    public abstract class ControllerClosedUpdateMode : ControllerUpdateMode
    {
        protected abstract ControllerUpdateMode ModeSource{get;}
        public override string Name => ModeSource.Name;
        public override Action OnAnimationEndAction =>ModeSource.OnAnimationEndAction;
        public override Action UpdateAction =>ModeSource.UpdateAction;
        public override Action OnFallingAction => ModeSource.OnFallingAction;
        public override Action OnLandingAction => ModeSource.OnLandingAction;
        public override Action EnterAction => ModeSource.EnterAction;
        public override Action ExitAction => ModeSource.ExitAction;
    }
    public abstract class RunningCharacterSettings : CharacterSettings
    {
        [Range(0, 1000000)]
        public float RunSpeed;
        [Range(0, 100)]
        public float MeleeAtkDistance;
    }
    public abstract class AIBehavior<TBehavior,TOwner>: MonoBehaviour where TBehavior : AIBehavior<TBehavior,TOwner>
        where TOwner:GameCharacter
    {
        protected abstract TOwner Owner {get;}
        public abstract class AIStateConverter
        {
            public abstract Action UpdateAction { get; }
            public abstract Action<HitPointSystem.DamageHitInfo> OnTakeDamage { get; }
            public abstract Action<GameCharacter> OnFindEnemy { get; }
            public abstract Action OnLostEnemy { get; }
            public abstract Action OnAnimationEnd { get; }
            public abstract Action OnEnterAction { get; }
            public abstract Action OnExitAction { get; }
            public abstract string Name { get; }
        }
        public class AIState:IAIState<TBehavior,TOwner>
        {
            protected AIState() { }
            public AIState(Action<TBehavior> UpdateAction,Action<TBehavior, HitPointSystem.DamageHitInfo> OnTakeDamage,
                Action<TBehavior, GameCharacter> OnFindEnemy,Action<TBehavior> OnLostEnemy,Action<TBehavior> OnAnimationEnd,Action<TBehavior> OnEnterAction,
                Action<TBehavior> OnExitAction,string Name)
            {
                this.UpdateAction = UpdateAction;
                this.OnTakeDamage = OnTakeDamage;
                this.OnFindEnemy = OnFindEnemy;
                this.OnLostEnemy = OnLostEnemy;
                this.OnAnimationEnd = OnAnimationEnd;
                this.OnEnterAction = OnEnterAction;
                this.OnExitAction = OnExitAction;
                this.Name = Name;
            }
            public Action<TBehavior> UpdateAction { get; }
            public Action<TBehavior, HitPointSystem.DamageHitInfo> OnTakeDamage { get; }
            public Action<TBehavior, GameCharacter> OnFindEnemy { get; }
            public Action<TBehavior> OnLostEnemy { get; }
            public Action<TBehavior> OnAnimationEnd { get; }
            public Action<TBehavior> OnEnterAction { get; }
            public Action<TBehavior> OnExitAction { get; }
            public string Name { get; }
        }
        protected void ChangeBehaviorState(IAIState<TBehavior, TOwner> state)
        {
            ConvertedState.OnExitAction();
            SetStartState(state);
        }
        protected void SetStartState(IAIState<TBehavior, TOwner> state)
        {
            currentState = state;
            ConvertedState.OnEnterAction();
        }
        protected void ChangeSubState(SubState<TBehavior,TOwner> state)
        {
            if (currentState is SubState<TBehavior, TOwner> subState)
            {
                subState.OnSubExitAction();
            }
            SetStartSubState(state);
        }
        protected void SetStartSubState(SubState<TBehavior, TOwner> state)
        {
            currentState = state;
        }
        protected IAIState<TBehavior, TOwner> currentState { get; private set; }
        public abstract AIStateConverter ConvertedState { get;}
        public abstract void OnFallingAction();
        public virtual void OnLandingAction()
        {
            if (currentState is FallingState<TBehavior, TOwner> state)
            {
                ChangeBehaviorState(state.NextState);
            }
        }
        protected virtual void Update()
        {
            ConvertedState.UpdateAction();
        }
        protected virtual void Awake()
        {
            Owner.HitPointSystem.OnTakeDamageEvent +=
                delegate(HitPointSystem.DamageHitInfo hitInfo) { ConvertedState.OnTakeDamage(hitInfo); };
            Owner.OnFallingEvent += OnFallingAction;
            Owner.OnLandingEvent += OnLandingAction;
        }
    }
    public abstract class SubState<TBehavior,TOwner>: IAIState<TBehavior, TOwner>
        where TBehavior : AIBehavior<TBehavior, TOwner> where TOwner : GameCharacter
    {
        protected SubState(IAIState<TBehavior, TOwner> NextState)
        {
            this.NextState = NextState;
        }
        public abstract Action OnSubExitAction { get; }
        public abstract Action<TBehavior> UpdateAction { get; }
        public abstract Action<TBehavior, HitPointSystem.DamageHitInfo> OnTakeDamage { get; }
        public abstract Action<TBehavior, GameCharacter> OnFindEnemy { get; }
        public abstract Action<TBehavior> OnLostEnemy { get; }
        public abstract Action<TBehavior> OnAnimationEnd { get; }
        public Action<TBehavior> OnEnterAction => NextState.OnEnterAction;
        public Action<TBehavior> OnExitAction =>
            delegate (TBehavior behavior)
        {
            OnSubExitAction();
            NextState.OnExitAction(behavior);
        };
        public abstract string Name { get; }
        public IAIState<TBehavior, TOwner> NextState { get; protected set; }
    }
    public abstract class PathMovingState<TBehavior, TOwner> : SubState<TBehavior,TOwner>
        where TBehavior:AIBehavior<TBehavior,TOwner> where TOwner:GameCharacter
    {
        protected PathMovingState(IAIState<TBehavior, TOwner> NextState) : base(NextState) { }
        public override Action<TBehavior, HitPointSystem.DamageHitInfo> OnTakeDamage => NextState.OnTakeDamage;
        public override Action<TBehavior, GameCharacter> OnFindEnemy => NextState.OnFindEnemy;
        public override Action<TBehavior> OnLostEnemy => NextState.OnLostEnemy;
        public override Action<TBehavior> OnAnimationEnd => delegate { };
        public override string Name => "PathMovingState";
    }
    public abstract class PathFindWaitingState<TBehavior,TOwner> : PathMovingState<TBehavior, TOwner>
        where TBehavior : AIBehavior<TBehavior, TOwner> where TOwner : GameCharacter
    {
        protected abstract class PathFindThreadState
        {
            private PathFindThreadState() { }
            public PathFindThreadState(GameCharacter Start)
            {
                this.Start = Start;
            }
            public bool isDone = false;
            public Vector2[] Path;
            public readonly GameCharacter Start;
        }
        private class PathFindPointThreadState : PathFindThreadState
        {
            public PathFindPointThreadState(GameCharacter Start, PathPoint End) : base(Start)
            {
                this.End = End;
            }
            public readonly PathPoint End;
        }
        private class PathFindCharacterThreadState : PathFindThreadState
        {
            public PathFindCharacterThreadState(GameCharacter Start, GameCharacter End) : base(Start)
            {
                this.End = End;
            }
            public readonly GameCharacter End;
        }
        private void StartPathFindThread(object threadState)
        {
            StartPathFindThread(threadState);
        }
        private void StartPathFindThread(PathFindCharacterThreadState threadState)
        {
            AutoResetEvent waitHandler = new AutoResetEvent(true);
            PathPoint start = null;
            PathPoint end = null;
            void destroyStartPoint()
            {
                GameObject.Destroy(start.gameObject);
                ResetThreadAction -= destroyStartPoint;
            }
            void destroyEndPoint()
            {
                GameObject.Destroy(end.gameObject);
                ResetThreadAction -= destroyEndPoint;
            }
            short id = Registry.Registry.ThreadManager.AddActionsQuery(new List<Action>
                {
                    delegate{start=(PathPoint)threadState.Start.GetNewPathPoint();ResetThreadAction+=destroyStartPoint; },
                    delegate{end=(PathPoint)threadState.End.GetNewPathPoint();ResetThreadAction+=destroyEndPoint;  }
                }, delegate { waitHandler.Set(); });
            ResetThreadAction += resetCreateTempPoint;
            void resetCreateTempPoint()
            {
                Registry.Registry.ThreadManager.RemoveActionsQueve(id);
                ResetThreadAction -= resetCreateTempPoint;
            }
            waitHandler.WaitOne();
            ResetThreadAction -= resetCreateTempPoint;
            PathFinding.PathFinding.DextraPathFinding pathFind =
                new PathFinding.PathFinding.DextraPathFinding(start, end);
            pathFind.AsyncFindWay();
            threadState.Path = pathFind.GetPath();
            Registry.Registry.ThreadManager.AddActionsQuery(new List<Action>
                {
                    delegate{GameObject.Destroy(start.gameObject); },
                    delegate{GameObject.Destroy(end.gameObject); }
                }, delegate { });
            ResetThreadAction -= destroyEndPoint;
            ResetThreadAction -= destroyStartPoint;
            threadState.isDone = true;
        }
        private void StartPathFindPointThread(object threadState)
        {
            StartPathFindPointThread((PathFindPointThreadState)threadState);
        }
        private void StartPathFindPointThread(PathFindPointThreadState threadState)
        {
            AutoResetEvent waitHandler = new AutoResetEvent(false);
            PathPoint start = null;
            short id = Registry.Registry.ThreadManager.AddActionsQuery(new List<Action>
                {
                    delegate{start=(PathPoint)threadState.Start.GetNewPathPoint();}
                }, delegate { waitHandler.Set(); });
            ResetThreadAction += resetCreateTempPoint;
            void resetCreateTempPoint()
            {
                Registry.Registry.ThreadManager.RemoveActionsQueve(id);
                ResetThreadAction -= resetCreateTempPoint;
            }
            void destroyTempPoint()
            {
                GameObject.Destroy(start.gameObject);
                ResetThreadAction -= destroyTempPoint;
            }
            waitHandler.WaitOne();
            ResetThreadAction += destroyTempPoint;
            ResetThreadAction -= resetCreateTempPoint;
            PathFinding.PathFinding.DextraPathFinding pathFind =
                new PathFinding.PathFinding.DextraPathFinding(start, threadState.End);
            pathFind.AsyncFindWay();
            threadState.Path = pathFind.GetPath();
            Registry.Registry.ThreadManager.AddActionsQuery(new List<Action>
            { delegate { GameObject.Destroy(start.gameObject); } },
            delegate { });
            threadState.isDone = true;
        }
        protected PathFindWaitingState(IAIState<TBehavior, TOwner> NextState, GameCharacter start, GameCharacter end) : base(NextState)
        {
            start.OnChangePlatformEvent += ReloadThreadAction;
            onExitAction += delegate
            {
                start.OnChangePlatformEvent -= ReloadThreadAction;
            };

            end.OnChangePlatformEvent += ReloadThreadAction;
            onExitAction += delegate
            {
                end.OnChangePlatformEvent -= ReloadThreadAction;
            };

            Registry.Registry.CurrentLevelPathMap.OnChangeMapEvent += ReloadThreadAction;
            onExitAction = delegate
            {
                Registry.Registry.CurrentLevelPathMap.OnChangeMapEvent -= ReloadThreadAction;
                ResetThreadAction();
            };

            ResetThreadAction = delegate
            {
                if (CurrentThread != null)
                {
                    CurrentThread.Abort();
                }
            };
            RestartThreadAction = delegate
            {
                CurrentThread = new Thread(new ParameterizedThreadStart(StartPathFindThread));
                CurrentThread.Start(threadState);
            };
            threadState = new PathFindCharacterThreadState(start, end);
            RestartThreadAction();
        }
        protected PathFindWaitingState(IAIState<TBehavior, TOwner> NextState, GameCharacter start, ITemporalPathPoint end) : base(NextState)
        {
            onExitAction += delegate
            {
                GameObject.Destroy(((PathPoint)end).gameObject);
            };

            start.OnChangePlatformEvent += ReloadThreadAction;
            onExitAction += delegate
            {
                start.OnChangePlatformEvent -= ReloadThreadAction;
            };

            Registry.Registry.CurrentLevelPathMap.OnChangeMapEvent += ReloadThreadAction;
            onExitAction+= delegate
            {
                Registry.Registry.CurrentLevelPathMap.OnChangeMapEvent -= ReloadThreadAction;
                ResetThreadAction();
            };

            ResetThreadAction = delegate
            {
                if (CurrentThread != null)
                {
                    CurrentThread.Abort();
                }
            };
            RestartThreadAction = delegate
            {
                CurrentThread = new Thread(new ParameterizedThreadStart(StartPathFindPointThread));
                CurrentThread.Start(threadState);
            };
            threadState = new PathFindPointThreadState(start,(PathPoint)end);
            RestartThreadAction();
        }
        private Thread CurrentThread;
        protected PathFindThreadState threadState;
        private Action ReloadThreadAction => ResetThreadAction + RestartThreadAction;
        private Action ResetThreadAction = delegate { };
        private Action RestartThreadAction = delegate { };
        public readonly Action onExitAction;
        public override Action OnSubExitAction => onExitAction;
    }
    public abstract class FallingState<TBehavior, TOwner> : PathMovingState<TBehavior, TOwner>
        where TBehavior : AIBehavior<TBehavior, TOwner> where TOwner : GameCharacter
    {
        public FallingState(IAIState<TBehavior,TOwner> NextState) : base(NextState) { }
        public override Action<TBehavior> UpdateAction => delegate { };
        public override Action OnSubExitAction => delegate { };
    }
    public abstract class Platform<TTempPathPointType> : MonoBehaviour, ITempPointInstantiator
        where TTempPathPointType:MonoBehaviour,ITemporalPathPoint
    {
        public abstract PathPoint FirstPoint { get; }
        public abstract PathPoint SecondPoint { get; }
        public abstract ITemporalPathPoint InstantiateTempPoint(Vector2 position);
        protected static TTempPathPointType InstantiateTempPoint(Vector2 position, GameObject tempPointPrefab,ITempPointInstantiator owner)
        {
            TTempPathPointType point = Instantiate(tempPointPrefab,position,Quaternion.Euler(Vector3.zero)).
                GetComponent<TTempPathPointType>();
            point.Owner = owner;
            point.Initialize();
            return point;
        }
    }
    public abstract class CollectibleItem : MonoBehaviour
    {
        protected abstract void CollectItem();
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<MainHero>() != null)
            {
                CollectItem();
            }
        }
    }
}
