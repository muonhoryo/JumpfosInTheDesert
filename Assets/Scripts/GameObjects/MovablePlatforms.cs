using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class MovablePlatforms : MonoBehaviour, IMovableObject
    {
        [SerializeField]
        private Rigidbody2D RGBody;
        public void Move(float moveSpeed, int direction)
        {
            RGBody.MoveObject(Vector2.right * direction, moveSpeed);
        }
    }
}
