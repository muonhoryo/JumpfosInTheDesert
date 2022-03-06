using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

namespace Registry
{
    [CreateAssetMenu]
    public class ConstData : ScriptableObject
    {
        public GameObject[] GameItems;
        public GameObject TempSimplePathPointPrefab;
        public GameObject TempStairsPathPointPrefab;
        public LayerMask WallLayers;
        public LayerMask HalfMovebleLayers;
        public LayerMask NotMovebleLayers;
        public float MovePointEndDistance;
        public float CameraStartMoveRadius;
        public float CameraLeftBorder;
        public float CameraRightDorder;
        public float CameraUpperBorder;
        public float CameraLowerBorder;
        public float ThreadManagerMaxTimeFrameWork;
    }
}

