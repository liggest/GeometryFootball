using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Test
{
    public class asyncTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("开始");
            ResourcesManager.Load("DemoPlayer", (GameObject g) =>
             {
                 Debug.Log(g);
                 Debug.Log(g.name);
             });

            //ResourcesManager.LoadAsync("Players", "DemoPlayer", (GameObject g) =>
            //  {
            //      Debug.Log(g);
            //      Debug.Log(g.name);
            //  });
            //Task<string> val = t1();
            //Debug.Log(val);
            Debug.Log("结束");
        }

        // Update is called once per frame
        void Update()
        {

        }


        async Task<string> t1()
        {
            string val = await t2();
            Debug.Log(val);
            return val;
        }


        async Task<string> t2()
        {
            Debug.Log("等一秒");
            await Task.Delay(1000);
            Debug.Log("等一秒后面");
            return "233";
        }
    }
}


