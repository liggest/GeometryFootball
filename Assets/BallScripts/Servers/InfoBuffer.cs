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
        private static readonly List<Vector3Holder> localPositionBuffer = new List<Vector3Holder>();
        private static readonly List<QuaternionHolder> rotationBuffer = new List<QuaternionHolder>();
        private static readonly List<QuaternionHolder> localRotationBuffer = new List<QuaternionHolder>();
        private static readonly List<Vector3Holder> localScaleBuffer = new List<Vector3Holder>();

        private void Awake()
        {
            positionBuffer.Clear();
            localPositionBuffer.Clear();
            rotationBuffer.Clear();
            localRotationBuffer.Clear();
            localScaleBuffer.Clear();
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
            lock (rotationBuffer)
            {
                rotationBuffer.Add(new QuaternionHolder { category = _category, id = _id, quat = rot });
            }
        }
        public static void AddLocal(StageObjectCategory _category, int _id, Vector3 localPos)
        {
            lock (localPositionBuffer)
            {
                localPositionBuffer.Add(new Vector3Holder { category = _category, id = _id, vect = localPos });
            }
        }
        public static void AddLocal(StageObjectCategory _category, int _id, Quaternion localRot)
        {
            lock (localRotationBuffer)
            {
                localRotationBuffer.Add(new QuaternionHolder { category = _category, id = _id, quat = localRot });
            }
        }
        public static void AddLocalScale(StageObjectCategory _category, int _id, Vector3 localScale)
        {
            lock (localScaleBuffer)
            {
                localScaleBuffer.Add(new Vector3Holder { category = _category, id = _id, vect = localScale });
            }
        }

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
            List<Vector3Holder> localPosList = ReleaseList(localPositionBuffer);
            List<QuaternionHolder> localRotList = ReleaseList(localRotationBuffer);
            List<Vector3Holder> localScaleList = ReleaseList(localScaleBuffer);
            if (posList!=null)
            {
                ServerSend.StageObjectPositions(posList);
            }
            if (rotList != null)
            {
                ServerSend.StageObjectRotations(rotList);
            }
            if (localPosList != null)
            {
                ServerSend.StageObjectLocalPositions(localPosList);
            }
            if (localRotList != null)
            {
                ServerSend.StageObjectLocalRotations(localRotList);
            }
            if (localScaleList != null)
            {
                ServerSend.StageObjectLocalScales(localScaleList);
            }
        }
    }
}

