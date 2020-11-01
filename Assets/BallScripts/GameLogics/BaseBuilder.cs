using System;
using System.Collections.Generic;

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
        /// <returns>GameObject上挂载的BaseStageObject</returns>
        public abstract T BuildObject(Tinfo info);
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
                UnityEngine.Debug.Log(GetType());
                return this;
            }
            if (Next != null)
            {
                return Next.GetCorrectBuilder(info);
            }
            return null;
        }
    }
}
