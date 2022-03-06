using Registry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GUI
{
    public sealed class MainCamera : MonoBehaviour, ISingltone<MainCamera>
    {
        private MainCamera Singltone;
        MainCamera ISingltone<MainCamera>.Singltone { get => Singltone; set => Singltone=value; }
        private System.Action UpdateAction;
        private Vector2 MHPosition => Registry.Registry.MHScript.transform.position;
        private float CameraLeftBorder => Registry.Registry.ÑonstData.CameraLeftBorder;
        private float CameraRightBorder => Registry.Registry.ÑonstData.CameraRightDorder;
        private float CameraUpperBorder => Registry.Registry.ÑonstData.CameraUpperBorder;
        private float CameraLowerBorder => Registry.Registry.ÑonstData.CameraLowerBorder;
        private float CameraStartMoveRadius => Registry.Registry.ÑonstData.CameraStartMoveRadius;
        private void SetDefaultMode()
        {
            UpdateAction = DefaultMove;
        }
        private void Awake()
        {
            SingltoneStatic.Awake(this, delegate { Destroy(this); }, SetDefaultMode);
        }
        private void DefaultMove()
        {
            float newX = transform.position.x;
            float newY = transform.position.y;
            {
                float xDiff = MHPosition.x - transform.position.x;
                float yDiff = MHPosition.y - transform.position.y;
                if (xDiff > CameraStartMoveRadius)
                {
                    newX = MHPosition.x - CameraStartMoveRadius;
                    if (newX > CameraRightBorder)
                    {
                        newX = CameraRightBorder;
                    }
                }
                else if (xDiff < -CameraStartMoveRadius)
                {
                    newX = MHPosition.x + CameraStartMoveRadius;
                    if (newX < CameraLeftBorder)
                    {
                        newX = CameraLeftBorder;
                    }
                }
                if (yDiff > CameraStartMoveRadius)
                {
                    newY = MHPosition.y - CameraStartMoveRadius;
                    if (newY > CameraUpperBorder)
                    {
                        newY = CameraUpperBorder;
                    }
                }
                else if (yDiff < -CameraStartMoveRadius)
                {
                    newY = MHPosition.y +CameraStartMoveRadius;
                    if (newY < CameraLowerBorder)
                    {
                        newY = CameraLowerBorder;
                    }
                }
            }
            transform.position = new Vector3(newX, newY, transform.position.z);
        }
        private void LateUpdate()
        {
            UpdateAction();
        }
    }
}
