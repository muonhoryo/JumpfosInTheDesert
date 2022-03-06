using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public class Coin : CollectibleItem
    {
        protected override void CollectItem()
        {
            Registry.Registry.CoinCount++;
            Debug.Log(Registry.Registry.CoinCount);
            Destroy(gameObject);
        }
    }
}
