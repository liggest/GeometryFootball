using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NetworkMarkerAttribute : Attribute
    {
        private string marker;

        public NetworkMarkerAttribute(string _marker)
        {
            marker = _marker;
        }

        public string Marker { get => marker; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class NetworkClassAttribute : Attribute
    {

        public NetworkClassAttribute()
        {

        }
    }
}
