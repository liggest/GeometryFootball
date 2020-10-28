using System;
using System.Collections.Generic;

namespace BallScripts.GameLogics
{
    public enum BuildType
    {
        Server,
        Client
    }

    public abstract class BaseBuilder<T> where T : BaseStageObject
    {
        public abstract T BuildObject(string prefabName);
        public abstract T BuildInClient(T obj);
        public abstract T BuildInServer(T obj);

        public Func<T, T> ExtraProcess;
        public T Build(string prefabName,BuildType type = BuildType.Client)
        {
            T obj = BuildObject(prefabName);
            T result;
            if (type == BuildType.Server)
            {
                result = BuildInServer(obj);
            }
            else
            {
                result = BuildInClient(obj);
            }
            if (ExtraProcess != null) 
            {
                result = ExtraProcess.Invoke(result);
            }
            return result;
        }
    }
}
