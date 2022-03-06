using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamingObjects;
using PathFinding;

namespace Registry
{
    public sealed class RegistryInitialization : MonoBehaviour,ISingltone<RegistryInitialization>
    {
        private static RegistryInitialization Singltone;
        RegistryInitialization ISingltone<RegistryInitialization>.Singltone { get => Singltone; set => Singltone = value; }
        [SerializeField]
        private PathPointList PathPoints;
        [SerializeField]
        private OttoSettings OttoSettings;
        [SerializeField]
        private MainHeroSettings MainHeroSettings;
        [SerializeField]
        private ConstData ConstData;
        [SerializeField]
        private MainHero MHScript;
        [SerializeField]
        private ThreadManager ThreadManager;
        private void SetConst()
        {
            Registry.ThreadManager = ThreadManager;
            Registry.ÑonstData = ConstData;
            Registry.MainHeroSettings = MainHeroSettings;
            Registry.OttoSettings = OttoSettings;
            Registry.MHScript = MHScript;
            Registry.CurrentLevelPathMap = new PathMap(PathPoints.Points);
            Registry.CurrentLevelPathMap.ConstructPathWays(
                PathPointPair.ConvertPointsArrays(PathPoints.StartSimplePoints, PathPoints.EndSimplePoints),
                PathPointPair.ConvertPointsArrays(PathPoints.StartStairsPoints,PathPoints.EndStairsPoints));
            Registry.LevelLayer = ConstData.NotMovebleLayers + ConstData.HalfMovebleLayers;
        }
        private void Awake()
        {
            SingltoneStatic.Awake(this,Start,SetConst);
        }
        private void Start()
        {
            Destroy(this);
        }
    }
}
