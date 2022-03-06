using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using PathFinding;

namespace GamingObjects
{
    public sealed class StepChecker : MonoBehaviour
    {
        //events
        public event Action OnChangePlatformEvent = delegate { };
        public event Action OnFallingEvent = delegate { };
        public event Action OnLandingEvent = delegate { };
        //others
        private Vector2 ColliderOffset;
        private Vector2 ColliderSize;
        [SerializeField]
        private GameCharacter GameCharacter;
        public ITempPointInstantiator CurrentPlatform;
        private int ImpassableCount;
        private int HalfPassableCount;
        private Action<Collider2D> OnTriggerExitAction;
        private PlatformTransitChecker transitChecker;
        private bool UpdateCollisionField(ref int field, int collisionLayerMask, int layerMask,
            Registry.Registry.ReferenceAction<int> countOperation)
        {
            if ((collisionLayerMask & layerMask) > 0)
            {
                countOperation(ref field);
                return true;
            }
            return false;
        }
        private void UpdateCollisionCount(Collider2D collision, Registry.Registry.ReferenceAction<int> countOperation)
        {
            int layer = collision.gameObject.layer.GetIntLayerMask();
            if (!UpdateCollisionField(ref ImpassableCount, layer, Registry.Registry.ÑonstData.NotMovebleLayers.value, countOperation))
            {
                UpdateCollisionField(ref HalfPassableCount, layer, Registry.Registry.ÑonstData.HalfMovebleLayers.value, countOperation);
            }
        }
        private void DefaultOnTriggerExit(Collider2D collision)
        {
            UpdateCollisionCount(collision, delegate (ref int field) { field--; });
            if (!GameCharacter.isFalling
                && (ImpassableCount <= 0 && (HalfPassableCount <= 0 || GameCharacter.IgnoringHalfMovebleLayers)))
            {
                OnFallingEvent();
            }
        }
        private void TransitOnTriggerExit(Collider2D collision)
        {
            UpdateCollisionCount(collision, delegate (ref int field) { field--; });
            if (ImpassableCount <= 0 && (HalfPassableCount <= 0 || GameCharacter.IgnoringHalfMovebleLayers))
            {
                OnTriggerExitAction = DefaultOnTriggerExit;
                Destroy(transitChecker);
            }
        }
        private void UpdateCurrentPlatform()
        {
            if (!GameCharacter.isFalling)
            {
                Collider2D coll = Physics2D.OverlapBox(ColliderOffset, ColliderSize, 0,
                    Registry.Registry.ÑonstData.NotMovebleLayers.value|
                    (!GameCharacter.IgnoringHalfMovebleLayers?Registry.Registry.ÑonstData.HalfMovebleLayers.value:0));
                if (coll != null)
                {
                    CurrentPlatform = coll.GetComponent<ITempPointInstantiator>();
                }
                else
                {
                    CurrentPlatform = null;
                }
            }
            else
            {
                CurrentPlatform = null;
            }
            OnChangePlatformEvent();
        }
        private void UpdateCurrentPlatform(Collider2D exitCollision)
        {
            if (CurrentPlatform == null || exitCollision.GetComponent<ITempPointInstantiator>() == CurrentPlatform)
            {
                UpdateCurrentPlatform();
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            OnTriggerExitAction(collision);
            UpdateCurrentPlatform(collision);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            UpdateCollisionCount(other, delegate (ref int field) { field++; });
            if (GameCharacter.isFalling && 
                (ImpassableCount > 0 || (!GameCharacter.IgnoringHalfMovebleLayers && HalfPassableCount > 0)))
            {
                if (GameCharacter.VerticalVelocity > 0)
                {
                    if (transitChecker == null)
                    {
                        OnTriggerExitAction = TransitOnTriggerExit;
                        transitChecker = gameObject.AddComponent<PlatformTransitChecker>();
                        transitChecker.Initialize(this, GameCharacter);
                    }
                }
                else
                {
                    OnLandingEvent();
                }
            }
            CurrentPlatform = other.GetComponent<ITempPointInstantiator>() ?? CurrentPlatform;
        }
        private void Awake()
        {
            BoxCollider2D coll = GetComponent<BoxCollider2D>();
            ColliderOffset = coll.offset;
            ColliderSize = coll.size;
            GameCharacter.OnPassabilityChangeEvent += PassabilityIsChanged;
            OnTriggerExitAction = DefaultOnTriggerExit;
            if (ImpassableCount <= 0 || (HalfPassableCount <= 0 && !GameCharacter.IgnoringHalfMovebleLayers))
            {
                OnFallingEvent();
            }
        }
        private void PassabilityIsChanged()
        {
            if (GameCharacter.IgnoringHalfMovebleLayers)
            {
                if (ImpassableCount <= 0)
                {
                    OnFallingEvent();
                }
                if (CurrentPlatform==null||
                    (((MonoBehaviour)CurrentPlatform).gameObject.layer & Registry.Registry.ÑonstData.HalfMovebleLayers) != 0)
                {
                    UpdateCurrentPlatform();
                }
            }
            else
            {
                if (GameCharacter.isFalling&&ImpassableCount > 0)
                {
                    OnLandingEvent();
                }
                if (CurrentPlatform == null)
                {
                    UpdateCurrentPlatform();
                }
            }
        }
        public void TransitIsDone()
        {
            OnTriggerExitAction = DefaultOnTriggerExit;
            Destroy(transitChecker);
            OnLandingEvent();
        }
    }
}
