using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using PathFinding;
using Registry;


namespace UnityEngine
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Return Vector2 direction of angle
        /// (up:0f, left:90f, down:180f, right:270f)
        /// </summary>
        /// <param name="DegAngle"></param>
        /// <returns></returns>
        public static Vector2 Direction(this float DegAngle)
        {
            return new Vector2(-Mathf.Sin(DegAngle*Mathf.Deg2Rad), Mathf.Cos(DegAngle*Mathf.Deg2Rad));
        }
        public static Vector2[] ConvertPath(this PathPoint[] path)
        {
            Vector2[] array = new Vector2[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                array[i] = path[i].transform.position;
            }
            return array;
        }
        public static Vector2[] AsyncConvertPath(this PathPoint[] path,ThreadManager.ThreadQueryReservator reservator)
        {
            Vector2[] array = new Vector2[path.Length];
            AutoResetEvent waitHandler = new AutoResetEvent(false);
            List<System.Action> list = new List<System.Action> { };
            for(int i = 0; i < path.Length; i++)
            {
                int j = i;
                list.Add(delegate { array[j] = path[j].transform.position; });
            }
            Registry.Registry.ThreadManager.AddActionsQuery(list, delegate { waitHandler.Set(); }, reservator);
            waitHandler.WaitOne();
            return array;
        }
        public static ITempPointInstantiator GetPlatformAtPoint(this Vector2 point)
        {
            return Physics2D.Raycast(point, Vector2.down, float.MaxValue, Registry.Registry.LevelLayer).
                collider.GetComponent<ITempPointInstantiator>();
        }
        public static int GetIntLayerMask(this int layer)
        {
            return (int)Mathf.Pow(2, layer);
        }
        public static void MoveObject(this Rigidbody2D movedObject,Vector2 direction,float speed)
        {
            movedObject.AddForce(direction * speed / Time.deltaTime, ForceMode2D.Impulse);
        }
    }
}
