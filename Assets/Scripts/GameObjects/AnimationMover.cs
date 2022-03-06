using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class AnimationMover : MonoBehaviour
    {
        [HideInInspector]
        public float MoveSpeed;
        [SerializeField]
        private GameCharacter GameCharacter;
        private void Update()
        {
            GameCharacter.Move(MoveSpeed);
        }
        private void Awake()
        {
            if (enabled) enabled = false;
        }
    }
}
