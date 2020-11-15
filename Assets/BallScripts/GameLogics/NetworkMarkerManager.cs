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
        public static Dictionary<ValueTuple<StageObjectCategory, int, string>, object> objCache = new Dictionary<(StageObjectCategory, int, string), object>();
        public static BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;


        public static void FindNetworkTypes(Type typeOfAssembly)
        {
            Type attrType = typeof(NetworkClassAttribute);
            Assembly assembly = typeOfAssembly.Assembly;
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.IsDefined(attrType))
                {
                    networkTypes.Add(type);
                    Debug.Log($"[NetworkMarkerManager]找到了类 {type.Name}");
                    FindNetworkMembers(type);
                }
            }
        }

        public static void FindNetworkMembers(Type networkType)
        {
            foreach (FieldInfo info in networkType.GetFields(bindingFlags))
            {
                NetworkMarkerAttribute attr = info.GetCustomAttribute<NetworkMarkerAttribute>(true);
                if (attr != null)
                {
                    networkMembers.Add(attr.Marker, info);
                    Debug.Log($"[NetworkMarkerManager]找到了标记 {attr.Marker} - 字段 {info.Name}");
                }
            }
            foreach (PropertyInfo info in networkType.GetProperties(bindingFlags)) 
            {
                NetworkMarkerAttribute attr = info.GetCustomAttribute<NetworkMarkerAttribute>(true);
                if (attr != null)
                {
                    networkMembers.Add(attr.Marker, info);
                    Debug.Log($"[NetworkMarkerManager]找到了标记 {attr.Marker} - 属性 {info.Name}");
                }
            }
            foreach (MethodInfo info in networkType.GetMethods(bindingFlags)) 
            {
                NetworkMarkerAttribute attr = info.GetCustomAttribute<NetworkMarkerAttribute>(true);
                if (attr != null)
                {
                    networkMembers.Add(attr.Marker, info);
                    Debug.Log($"[NetworkMarkerManager]找到了标记 {attr.Marker} - 方法 {info.Name}");
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

        public static void InvokeMethod(string marker, object obj, params object[] parameters)
        {
            if (networkMembers.TryGetValue(marker, out MemberInfo minfo))
            {
                if (minfo.MemberType == MemberTypes.Method)
                {
                    MethodInfo info = minfo as MethodInfo;
                    info.Invoke(obj, parameters);
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

        public static void SetStageObjectInfo(StageObjectCategory category, int id, string route, object value, bool useCache = true)
        {
            string[] routes = route.Split(new char[] { '.', '/' });
            string marker = routes[routes.Length - 1];
            ValueTuple<StageObjectCategory, int, string> pair = (category, id, route);
            if (useCache && objCache.TryGetValue(pair, out object obj))
            {
                SetValue(marker, obj, value);
            }
            else
            {
                BaseStageObject root = StageManager.instance.GetStageObject(category, id);
                if (root)
                {
                    obj = GetObj(root, routes);
                    objCache[pair] = obj;
                    SetValue(marker, obj, value);
                }
            }
        }

        public static void CallMethod(object root, string route, params object[] parameters)
        {
            string[] routes = route.Split(new char[] { '.', '/' });
            string marker = routes[routes.Length - 1];
            object obj = GetObj(root, routes);
            InvokeMethod(marker, obj, parameters);
        }

        public static void CallStageObjectMethod(StageObjectCategory category, int id, string route, bool useCache = true, params object[] parameters)
        {
            string[] routes = route.Split(new char[] { '.', '/' });
            string marker = routes[routes.Length - 1];
            ValueTuple<StageObjectCategory, int, string> pair = (category, id, route);
            if (useCache && objCache.TryGetValue(pair, out object obj))
            {
                InvokeMethod(marker, obj, parameters);
            }
            else
            {
                BaseStageObject root = StageManager.instance.GetStageObject(category, id);
                if (root)
                {
                    obj = GetObj(root, routes);
                    objCache[pair] = obj;
                    InvokeMethod(marker, obj, parameters);
                }
            }
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

