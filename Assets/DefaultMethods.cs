using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GamingObjects
{
    public static class SpeedModeCharacter
    {
        public static void ChangeMoveSpeed<TmoveModeEnum>(ISpeedModeCharacter<TmoveModeEnum> owner,int mode)
            where TmoveModeEnum : Enum
        {
            owner.MoveSpeed = owner.MoveSpeedConsts[mode]();
        }
    }
}
namespace PathFinding
{
    public static class TemporalPathPoint
    {
        public static void Initialize(ITemporalPathPoint point)
        {
            if (!point.isInitialized)
            {
                point.CreateNewWay(point.Owner.FirstPoint);
                point.CreateNewWay(point.Owner.SecondPoint);
                point.isInitialized = true;
            }
        }
    }
}
namespace Registry
{
    public static class SingltoneStatic
    {
        public static void Awake<TSingltoneType>(ISingltone<TSingltoneType> script,Action destroyAction,
            Action awakeAction)
            where TSingltoneType:UnityEngine.Object
        {
            if (script.Singltone != null)
            {
                destroyAction();
            }
            else
            {
                script.Singltone = (TSingltoneType)script;
                awakeAction();
            }
        }
    }
}
