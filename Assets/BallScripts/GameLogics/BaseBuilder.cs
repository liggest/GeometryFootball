using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public enum BuildType
    {
        Server,
        Client
    }

    public abstract class BaseBuilder<T,Tinfo> :IBuilderNode
        where T : BaseStageObject 
        where Tinfo : BaseBuildInfo
    {
        /// <summary>
        /// Builder链的下一项，用于在GetCorrectBuilder中遍历所有Builder
        /// </summary>
        public IBuilderNode Next { get ; set ; }
        /// <summary>
        /// 检查BuildInfo是否该由本Builder处理。用于在GetCorrectBuilder中得到正确的Builder
        /// </summary>
        /// <param name="info">给定的BuildInfo</param>
        /// <returns></returns>
        public abstract bool CheckInfo(BaseBuildInfo info);
        /// <summary>
        /// 通过BuildInfo构建GameObject
        /// </summary>
        /// <param name="info">给定的BuildInfo</param>
        /// <returns></returns>
        public virtual T BuildObject(Tinfo info)
        {
            GameObject obj = InstantiateByInfo(info);
            if (obj)
            {
                return AfterInstantiate(obj, info);
            }
            return null;
        }
        /// <summary>
        /// BuildObject方法中实例化后的具体步骤
        /// </summary>
        /// <param name="obj">实例化得到的GameObject</param>
        /// <param name="info">给定的BuildInfo</param>
        /// <returns></returns>
        public virtual T AfterInstantiate(GameObject obj, Tinfo info)
        {
            T stageObject = obj.GetComponent<T>();
            stageObject.Init(info.category, info.id);
            stageObject.prefabName = info.prefabName;
            stageObject.builder = this;
            return stageObject;
        }
        /// <summary>
        /// 对构建的BaseStageObject针对客户端进一步处理
        /// </summary>
        /// <param name="obj">构建的BaseStageObject</param>
        /// <param name="info">给定的BuildInfo</param>
        /// <returns></returns>
        public abstract T BuildInClient(T obj, Tinfo info);
        /// <summary>
        /// 对构建的BaseStageObject针对服务端进一步处理
        /// </summary>
        /// <param name="obj">构建的BaseStageObject</param>
        /// <param name="info">给定的BuildInfo</param>
        /// <returns></returns>
        public abstract T BuildInServer(T obj, Tinfo info);
        /// <summary>
        /// 给定BuildInfo和BuildType，构建BaseStageObject
        /// </summary>
        /// <param name="info">给定的BuildInfo</param>
        /// <param name="type">给定的BuildType</param>
        /// <returns></returns>
        public BaseStageObject Build(BaseBuildInfo info, BuildType type = BuildType.Client)
        {
            return Build((Tinfo)info, type);
        }
        /// <summary>
        /// 给定BuildInfo和BuildType，构建BaseStageObject
        /// </summary>
        /// <param name="info">给定的BuildInfo</param>
        /// <param name="type">给定的BuildType</param>
        /// <returns></returns>
        public T Build(Tinfo info, BuildType type = BuildType.Client)
        {
            T obj = BuildObject(info);
            T result;
            if (type == BuildType.Server)
            {
                result = BuildInServer(obj, info);
            }
            else
            {
                result = BuildInClient(obj, info);
            }
            return result;
        }

        /// <summary>
        /// 通过给定BuildInfo，得到正确的Builder
        /// </summary>
        /// <param name="info">给定的BuildInfo</param>
        /// <returns></returns>
        public IBuilderNode GetCorrectBuilder(BaseBuildInfo info)
        {
            if (CheckInfo(info))
            {
                Debug.Log($"由 {GetType().Name} 构建对象");
                return this;
            }
            if (Next != null)
            {
                return Next.GetCorrectBuilder(info);
            }
            return null;
        }
        /// <summary>
        /// 通过BuildInfo实例化Prefab（初始化位置信息）
        /// </summary>
        /// <param name="info">给定的BuildInfo</param>
        /// <returns></returns>
        public GameObject InstantiateByInfo(Tinfo info)
        {
            Debug.Log($"由{GetType().Name} 创建 {info.category} - {info.id}");
            GameObject prefab = ResourcesManager.Get<GameObject>(info.prefabName);
            if (prefab)
            {
                if (info.position == null && info.rotation == null) 
                {
                    return Object.Instantiate(prefab);
                }
                else
                {
                    Vector3 pos = info.position == null ? Vector3.zero : info.position.Value;
                    Quaternion rot = info.rotation == null ? Quaternion.identity : info.rotation.Value;
                    return Object.Instantiate(prefab, pos, rot);
                }
            }
            else
            {
                Debug.Log($"没有找到名为{info.prefabName}的Prefab");
            }
            return null;
        }
        /// <summary>
        /// 得到能够构建出给定StageObject的BuildInfo，通用方法
        /// </summary>
        /// <param name="obj">给定的StageObject</param>
        /// <returns></returns>
        public BaseBuildInfo GenerateBuildInfo(BaseStageObject obj)
        {
            if (obj.prefabName == string.Empty)
            {
                Debug.Log("StageObject的Prefab信息缺失，无法生成info");
                return null;
            }
            return GenerateInfo((T)obj);
        }
        /// <summary>
        /// 得到能够构建出给定StageObject的BuildInfo，被泛型具体化了的方法
        /// </summary>
        /// <param name="obj">给定的StageObject</param>
        /// <returns></returns>
        public abstract Tinfo GenerateInfo(T obj);
        /// <summary>
        /// 通过StageObject填充BuildInfo的快捷函数
        /// </summary>
        /// <param name="obj">给定的StageObject</param>
        /// <returns></returns>
        public Tinfo SetBaseInfo(Tinfo info, T obj)
        {
            info.category = obj.category;
            info.id = obj.id;
            info.position = obj.transform.position;
            info.rotation = obj.transform.rotation;
            info.prefabName = obj.prefabName;
            return info;
        }
    }
}
