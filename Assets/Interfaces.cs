using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GamingObjects;

namespace GamingObjects
{
    /// <summary>
    /// For character with more then one move mode
    /// </summary>
    /// <typeparam name="TMoveModeEnum"></typeparam>
    public interface ISpeedModeCharacter<TMoveModeEnum> where TMoveModeEnum:Enum
    {
        public Func<float>[] MoveSpeedConsts { get; }
        public float MoveSpeed { get; set; }
        public void ChangeMoveSpeed(int moveMode);
    }
    public interface IAIState<TBehavior,TOwner> where TBehavior:AIBehavior<TBehavior,TOwner> where TOwner:GameCharacter
    {
        public Action<TBehavior> UpdateAction { get; }
        public Action<TBehavior, HitPointSystem.DamageHitInfo> OnTakeDamage { get; }
        public Action<TBehavior, GameCharacter> OnFindEnemy { get; }
        public Action<TBehavior> OnLostEnemy { get; }
        public Action<TBehavior> OnAnimationEnd { get; }
        public Action<TBehavior> OnEnterAction { get; }
        public Action<TBehavior> OnExitAction { get; }
        public string Name { get; }
    }
    public interface IMovableObject
    {
        public void Move(float moveSpeed, int direction);
    }
}
namespace PathFinding
{
    public interface ITemporalPathPoint
    {
        public bool isInitialized { get; set; }
        public ITempPointInstantiator Owner { get; set; }
        public void CreateNewWay(PathPoint endPoint);
        public void Initialize();
    }
    public interface ITempPointInstantiator
    {
        public PathPoint FirstPoint { get; }
        public PathPoint SecondPoint { get; }
        public ITemporalPathPoint InstantiateTempPoint(Vector2 position);
    }
}
namespace Registry
{
    public interface ISingltone<TSingltoneType> where TSingltoneType:UnityEngine.Object
    {
        public TSingltoneType Singltone { get; set; }
    }
}
