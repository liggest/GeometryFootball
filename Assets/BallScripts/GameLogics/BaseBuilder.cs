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

        public T Build(string prefabName,BuildType type = BuildType.Client)
        {
            T obj = BuildObject(prefabName);
            if (type == BuildType.Server)
            {
                return BuildInServer(obj);
            }
            else
            {
                return BuildInClient(obj);
            }
        }
    }
}
