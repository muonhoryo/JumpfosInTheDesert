using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GamingObjects;
using PathFinding;

namespace Registry
{
    public static class Registry
    {
        public delegate void ReferenceAction<T>(ref T field);

        public static LayerMask LevelLayer;
        public static MainHero MHScript;
        public static ConstData ÑonstData;
        public static MainHeroSettings MainHeroSettings;
        public static OttoSettings OttoSettings;
        public static ThreadManager ThreadManager;
        private static int coinCount=0;
        public static int CoinCount
        {
            get => coinCount;
            set
            {
                CoinCountUpdateEvent(value);
                coinCount = value;
            }
        }
        public static event Action<int> CoinCountUpdateEvent = delegate { };
        private static PathMap currentLevelPathMap;
        public static PathMap CurrentLevelPathMap
        {
            get => currentLevelPathMap;
            set
            {
                if (currentLevelPathMap != null)
                {
                    currentLevelPathMap.OnMapReselected();
                }
                currentLevelPathMap = value;
            }
        }
    }
}
