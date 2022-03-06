using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GamingObjects
{
    public sealed class WallChecker : MonoBehaviour
    {
        [SerializeField]
        private float CheckedWallsDirection;
        [SerializeField]
        private MainHero Owner;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((collision.gameObject.layer.GetIntLayerMask() & Registry.Registry.ÑonstData.WallLayers) != 0)
            {
                Owner.OnSlideWall(CheckedWallsDirection);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if ((collision.gameObject.layer.GetIntLayerMask() & Registry.Registry.ÑonstData.WallLayers) != 0)
            {
                Owner.OnEndSlideWall();
            }
        }
    }
}
