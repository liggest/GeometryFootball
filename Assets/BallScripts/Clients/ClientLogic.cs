using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;
using System;

namespace BallScripts.Clients {
    public class ClientLogic
    {
        public static void BeginSceneLoading(string sceneName)
        {
            GameManager.instance.BeginLoadScene(sceneName,(string s)=>
            {
                if (s == sceneName)
                {
                    ClientSend.SceneLoaded(s);
                }
            });
        }

    }

}
