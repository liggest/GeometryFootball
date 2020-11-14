using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.Utils
{
    public enum PacketValueType
    {
        ByteValue=1,
        ShortValue,
        IntValue,
        LongValue,
        FloatValue,
        BoolValue,
        StringValue,
        Vector3Value,
        QuaternionValue,
        ListStart,
        ListEnd
    }

    public static class PacketExtension
    {

        public static void Write(this Packet packet,object value)
        {
            if(value is byte)
            {
                packet.Write((byte)PacketValueType.ByteValue);
                packet.Write((byte)value);
            }
            else if(value is short)
            {
                packet.Write((byte)PacketValueType.ShortValue);
                packet.Write((short)value);
            }
            else if (value is int)
            {
                packet.Write((byte)PacketValueType.IntValue);
                packet.Write((int)value);
            }
            else if (value is long)
            {
                packet.Write((byte)PacketValueType.LongValue);
                packet.Write((long)value);
            }
            else if (value is float)
            {
                packet.Write((byte)PacketValueType.FloatValue);
                packet.Write((float)value);
            }
            else if (value is bool)
            {
                packet.Write((byte)PacketValueType.BoolValue);
                packet.Write((bool)value);
            }
            else if (value is string)
            {
                packet.Write((byte)PacketValueType.StringValue);
                packet.Write((string)value);
            }
            else if (value is Vector3)
            {
                packet.Write((byte)PacketValueType.Vector3Value);
                packet.Write((Vector3)value);
            }
            else if (value is Quaternion)
            {
                packet.Write((byte)PacketValueType.QuaternionValue);
                packet.Write((Quaternion)value);
            }
            else if(value is IList)
            {
                packet.Write((byte)PacketValueType.ListStart);
                IList l = value as IList;
                foreach (object item in l)
                {
                    packet.Write(item);
                }
                packet.Write((byte)PacketValueType.ListEnd);
            }
        }

        public static object ReadObject(this Packet packet, bool _moveReadPos = true)
        {
            PacketValueType valueType = (PacketValueType)packet.ReadByte();
            switch (valueType)
            {
                case PacketValueType.ByteValue:
                    return packet.ReadByte(_moveReadPos);
                case PacketValueType.ShortValue:
                    return packet.ReadShort(_moveReadPos);
                case PacketValueType.IntValue:
                    return packet.ReadInt(_moveReadPos);
                case PacketValueType.LongValue:
                    return packet.ReadLong(_moveReadPos);
                case PacketValueType.FloatValue:
                    return packet.ReadFloat(_moveReadPos);
                case PacketValueType.BoolValue:
                    return packet.ReadBool(_moveReadPos);
                case PacketValueType.StringValue:
                    return packet.ReadString(_moveReadPos);
                case PacketValueType.Vector3Value:
                    return packet.ReadVector3(_moveReadPos);
                case PacketValueType.QuaternionValue:
                    return packet.ReadQuaternion(_moveReadPos);
                case PacketValueType.ListStart:
                    ArrayList objs = new ArrayList();
                    object temp = packet.ReadObject(_moveReadPos);
                    while (temp != null)
                    {
                        objs.Add(temp);
                        temp = packet.ReadObject(_moveReadPos);
                    }
                    return objs.ToArray();
                case PacketValueType.ListEnd:
                default:
                    return null;
            }
        }
    }

}

