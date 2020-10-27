using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public struct Vector3Holder
    {
        public StageObjectCategory category;
        public int id;
        public Vector3 vect;
    }

    public struct QuaternionHolder
    {
        public StageObjectCategory category;
        public int id;
        public Quaternion quat;
    }

    public class InfoBuffer : MonoBehaviour
    {
        private static readonly List<Vector3Holder> positionBuffer = new List<Vector3Holder>();
        //private static readonly List<Vector3Holder> localPositionBuffer = new List<Vector3Holder>();
        private static readonly List<QuaternionHolder> rotationBuffer = new List<QuaternionHolder>();
        //private static readonly List<QuaternionHolder> localRotationBuffer = new List<QuaternionHolder>();

        private void Awake()
        {
            positionBuffer.Clear();
            rotationBuffer.Clear();
        }
        private void FixedUpdate()
        {
            UpdateMain();
        }

        public static void Add(StageObjectCategory _category, int _id, Vector3 pos)
        {
            lock (positionBuffer)
            {
                positionBuffer.Add(new Vector3Holder { category = _category, id = _id, vect = pos });
            }
        }
        public static void Add(StageObjectCategory _category, int _id, Quaternion rot)
        {
            lock (positionBuffer)
            {
                rotationBuffer.Add(new QuaternionHolder { category = _category, id = _id, quat = rot });
            }
        }
        /*
        public static void AddLocal(StageObjectCategory _category, int _id, Vector3 pos)
        {
            lock (positionBuffer)
            {
                localPositionBuffer.Add(new Vector3Holder { category = _category, id = _id, vect = pos });
            }
        }
        public static void AddLocal(StageObjectCategory _category, int _id, Quaternion rot)
        {
            lock (positionBuffer)
            {
                localRotationBuffer.Add(new QuaternionHolder { category = _category, id = _id, quat = rot });
            }
        }
        */

        public static List<T> ReleaseList<T>(List<T> origin)
        {
            if (origin.Count > 0)
            {
                List<T> result = new List<T>();
                lock (origin)
                {
                    result.AddRange(origin);
                    origin.Clear();
                }
                return result;
            }
            return null;
        }

        public static void UpdateMain()
        {
            List<Vector3Holder> posList = ReleaseList(positionBuffer);
            List<QuaternionHolder> rotList = ReleaseList(rotationBuffer);
            if (posList!=null)
            {
                ServerSend.StageObjectPosition(posList);
            }
            if (rotList != null)
            {
                ServerSend.StageObjectRotation(rotList);
            }
        }
    }
}

