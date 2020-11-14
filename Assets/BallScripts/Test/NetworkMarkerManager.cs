using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace BallScripts.GameLogics
{
    public class NetworkMarkerManager
    {
        public static Dictionary<string, MemberInfo> networkMembers = new Dictionary<string, MemberInfo>();
        public static HashSet<Type> networkTypes = new HashSet<Type>();

        public static void FindNetworkTypes(Type typeOfAssembly)
        {
            Assembly assembly = typeOfAssembly.Assembly;
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.GetCustomAttribute<NetworkClassAttribute>() != null)
                {
                    networkTypes.Add(type);
                    Debug.Log($"[NetworkMarkerManager]找到了类 {type.Name}");
                    FindNetworkMembers(type);
                }
            }
        }

        public static void FindNetworkMembers(Type networkType)
        {
            foreach (FieldInfo info in networkType.GetFields())
            {
                NetworkMarkerAttribute attr = info.GetCustomAttribute<NetworkMarkerAttribute>(true);
                if (attr != null)
                {
                    networkMembers.Add(attr.Marker, info);
                    Debug.Log($"[NetworkMarkerManager]找到了标记 {attr.Marker}");
                }
            }
            foreach (PropertyInfo info in networkType.GetProperties()) 
            {
                NetworkMarkerAttribute attr = info.GetCustomAttribute<NetworkMarkerAttribute>(true);
                if (attr != null)
                {
                    networkMembers.Add(attr.Marker, info);
                    Debug.Log($"[NetworkMarkerManager]找到了属性 {info.Name}");
                }
            }
        }

        public static void SetValue(string marker, object obj, object value)
        {
            if(networkMembers.TryGetValue(marker,out MemberInfo minfo))
            {
                if (minfo.MemberType == MemberTypes.Field)
                {
                    FieldInfo info = minfo as FieldInfo;
                    info.SetValue(obj, value);
                }else if(minfo.MemberType == MemberTypes.Property)
                {
                    PropertyInfo info = minfo as PropertyInfo;
                    info.SetValue(obj, value);
                }
            }
        }

        public static Type IListType = typeof(IList);
        public static Type IDictionaryType = typeof(IDictionary);

        public static object GetObj(object root,string[] routes)
        {
            Type currentType = root.GetType();
            object current = root;
            for (int i = 0; i < routes.Length - 1; i++)
            {
                if(routes[i].StartsWith("[") && routes[i].EndsWith("]"))
                {
                    string idx = routes[i].TrimStart('[').TrimEnd(']');
                    if (IListType.IsAssignableFrom(currentType) && int.TryParse(idx,out int numIdx))
                    {
                        current = (current as IList)[numIdx];
                    }
                    else if(IDictionaryType.IsAssignableFrom(currentType)){
                        current = (current as IDictionary)[idx];
                    }
                    else
                    {
                        current = null;
                    }
                }
                else
                {
                    FieldInfo fInfo = currentType.GetField(routes[i]);
                    if (fInfo != null)
                    {
                        current = fInfo.GetValue(current);
                    }
                    else
                    {
                        PropertyInfo pInfo = currentType.GetProperty(routes[i]);
                        if (pInfo != null)
                        {
                            current = pInfo.GetValue(current);
                        }
                        else
                        {
                            current = null;
                        }
                    }
                }
                if (current == null)
                {
                    break;
                }
                currentType = current.GetType();
            }
            Debug.Log($"current:{current}");
            return current;
        }

        public static object GetObj(object root,string route)
        {
            string[] routes = route.Split(new char[] { '.', '/' });
            return GetObj(root, routes);
        }

        public static void SetInfo(object root, string route, object value)
        {
            string[] routes = route.Split(new char[] { '.', '/' });
            string marker = routes[routes.Length - 1];
            object obj = GetObj(root, routes);
            SetValue(marker, obj, value);
        }

        public static string GetRoute(params string[] objs)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < objs.Length; i++)
            {
                names.Add(objs[i]);
            }
            Debug.Log(string.Join(".", names));
            return string.Join(".", names);
        }
    }
}

