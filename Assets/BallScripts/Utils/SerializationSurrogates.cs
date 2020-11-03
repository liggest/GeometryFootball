using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace BallScripts.Utils
{
    public class SurrogateManager
    {
        public static SurrogateSelector ss = null;
        static StreamingContext context;
        public static SurrogateSelector GetSurrogateSelector()
        {
            if (ss == null)
            {
                ss = new SurrogateSelector();
                context = new StreamingContext(StreamingContextStates.All);
                ss.AddSurrogate(typeof(Vector3), context, new Vector3SerializationSurrogate());
                ss.AddSurrogate(typeof(Quaternion), context, new QuaternionSerializationSurrogate());
            }
            return ss;
        }
    }

    sealed class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        // Method called to serialize a Vector3 object
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 vect = (Vector3)obj;
            info.AddValue("x", vect.x);
            info.AddValue("y", vect.y);
            info.AddValue("z", vect.z);
            //Debug.Log(v3);
        }

        // Method called to deserialize a Vector3 object
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 vect = (Vector3)obj;
            Type floatType = typeof(float);
            vect.x = (float)info.GetValue("x", floatType);
            vect.y = (float)info.GetValue("y", floatType);
            vect.z = (float)info.GetValue("z", floatType);
            obj = vect;
            return obj;   // Formatters ignore this return value //Seems to have been fixed!
        }
    }

    sealed class QuaternionSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Quaternion quat = (Quaternion)obj;
            info.AddValue("x", quat.x);
            info.AddValue("y", quat.y);
            info.AddValue("z", quat.z);
            info.AddValue("w", quat.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Quaternion quat = (Quaternion)obj;
            Type floatType = typeof(float);
            quat.x = (float)info.GetValue("x", floatType);
            quat.y = (float)info.GetValue("y", floatType);
            quat.z = (float)info.GetValue("z", floatType);
            quat.w = (float)info.GetValue("w", floatType);
            obj = quat;
            return obj;
        }
    }
}
