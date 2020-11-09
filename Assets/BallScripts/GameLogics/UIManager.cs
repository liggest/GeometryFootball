using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System;
using BallScripts.Utils;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace BallScripts.GameLogics
{
    [System.Serializable]
    public struct UIElement
    {
        public string name;
        public UIBehaviour UIObject;
    }

    public class UIManager : Singleton<UIManager>
    {
        [Tooltip("本场地的所有UI")]
        public List<UIElement> uiElements = new List<UIElement>();

        //从所有 Panel 那里汇总到的所有 UI
        protected Dictionary<string, Dictionary<string, List<UIBehaviour>>> allUI = new Dictionary<string, Dictionary<string, List<UIBehaviour>>>();


        public GameObject serverClient;
        public GameObject clientMenu;
        public GameObject serverMenu;
        public GameObject serverInfo;

        public InputField usernameField;
        public InputField ipField;
        public InputField clientPortField;

        public InputField serverPortField;

        public Text ipPortInfo;
        public Text clientCount;

        public GameObject m_CanvasRoot;
        public Dictionary<string, GameObject> m_PanelList = new Dictionary<string, GameObject>();

        private void Start()
        {

            if (Servers.Server.state == Servers.ServerState.Offline)
            {
                InitUI();
                GameManager.instance.ToString();
            }
            else
            {
                ToServerMenu();
                ToServerInfo();
            }

           

         InputField usernameField;
        InputField ipField;
         InputField clientPortField;

         InputField serverPortField;

        Text ipPortInfo;
        Text clientCount;


            //Debug.Log(GetOneUIComponent<Button>("ServerClient", "Server").name);
            foreach (string key in allUI.Keys)
            {
                Debug.LogFormat("key: {0} value{1}", key, allUI[key]);
            }

        }

        #region UIManager 基础函数
        private bool CheckCanvasRootIsNull() //判断canvas是不是null
        {
            if (m_CanvasRoot == null)
            {
                Debug.Log("m_CanvasRoot是空的，请给你的Canvas增加UIRootHandler.cs");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsPanelLive(string name) //判断界面是否开启
        {
            return m_PanelList.ContainsKey(name); //通过这个界面是否在dictionary里来判断
        }

        public void ShowPanel(string name)
        { //开启界面
            if (CheckCanvasRootIsNull())
            {
                return;
            }

            if (IsPanelLive(name))
            {
                Debug.LogErrorFormat("[{0}]已经开启，如果你想显示，请先关闭", name);
                return;
            }

            ResourcesManager.LoadAndInstantiate(name, m_CanvasRoot.transform, (panel) =>
            {
                panel.name = name;
                m_PanelList.Add(name, panel);
            });
        }

        public void TogglePanel(string name, bool isOn)//显示或隐藏界面
        {
            if (IsPanelLive(name))
            {
                if (m_PanelList[name] != null)
                {
                    m_PanelList[name].SetActive(isOn); //isOn决定现实还是隐藏
                }
                else
                {
                    Debug.LogErrorFormat("" +
                        "TogglePanel[{0}] 找不到", name);
                }
            }
        }

        public void ClosePanel(string name) //关闭界面
        {
            if (IsPanelLive(name))
            {
                if (m_PanelList[name] != null)
                {
                    Destroy(m_PanelList[name]);
                    m_PanelList.Remove(name);
                }
                else
                {
                    Debug.LogErrorFormat("" +
                        "ClosePanel[{0}] 找不到", name);
                }
            }
        }

        public void CloseAllPanel() //关闭所有界面
        {
            foreach (KeyValuePair<string, GameObject> item in m_PanelList)
            {
                if (item.Value != null)
                {
                    Destroy(item.Value);
                }
            }
            m_PanelList.Clear();
        }

        public Vector2 GetCanvasSize() //获得Canvas大小
        {
            if (CheckCanvasRootIsNull())
            {
                return Vector2.one * -1;
            }

            RectTransform trans = m_CanvasRoot.transform as RectTransform;

            return trans.sizeDelta;
        }
        #endregion


        public void InitUI()
        {
            serverClient.SetActive(true);
            clientMenu.SetActive(false);
            serverMenu.SetActive(false);
            serverInfo.SetActive(false);

            serverPortField.interactable = true;
            clientPortField.interactable = true;
            ipField.interactable = true;
            usernameField.interactable = true;

        }

        public void ToServerMenu()
        {
            serverClient.SetActive(false);
            serverMenu.SetActive(true);
        }

        public void ToClientMenu()
        {
            serverClient.SetActive(false);
            clientMenu.SetActive(true);
        }

        public void ToServerInfo()
        {
            serverMenu.SetActive(false);
            serverInfo.SetActive(true);
        }

        public void BeServer()
        {
            int port = 6960;
            if (serverPortField.text == "" || IsIntBetween(serverPortField.text, 0, 65535, out port)) 
            {
                serverPortField.interactable = false;
                Servers.Actions.PlayerCountUpatedAction += SetClientCount;
                GameManager.instance.BeServer(port);
                ipPortInfo.text = $"{IPAddress.Any}:{port}";
                ToServerInfo();
            }
            else
            {
                Debug.Log("端口有误，请填入0-65535的整数");
            }
        }

        public void BeClient()
        {
            string ip = "127.0.0.1";
            int port = 6960;
            string name = "Anonymous";
            if (clientPortField.text != "" && !IsIntBetween(clientPortField.text, 0, 65535, out port))
            {
                Debug.Log("端口好像不太对，请填入0-65535的整数");
                return;
            }
            if (IsIP(ipField.text)) 
            {
                ip = ipField.text;
            }
            else if(ipField.text != "")
            {
                Debug.Log("IP格式好像不太对，请检查");
                return;
            }
            if (usernameField.text != "")
            {
                name = usernameField.text;
            }
            clientMenu.SetActive(false);
            clientPortField.interactable = false;
            ipField.interactable = false;
            usernameField.interactable = false;
            GameManager.instance.BeClient(ip, port, name);
            Clients.Actions.DisconnectedAction += OnClientDisconnect;
        }

        public void OnClientDisconnect()
        {
            clientMenu.SetActive(true);
            clientPortField.interactable = true;
            ipField.interactable = true;
            usernameField.interactable = true;
            Clients.Actions.DisconnectedAction -= OnClientDisconnect;
        }

        /*
        public void ConnectToServer()
        {
            startMenu.SetActive(false);
            usernameField.interactable = false;
            Client.instance.ConnectToServer();
        }
        */

        public void SetClientCount(int value)
        {
            clientCount.text = $"{Servers.Server.PlayerCount}/{Servers.Server.MaxPlayers}";
        }

        public static bool IsIntBetween(string text,int min,int max,out int value)
        {
            return int.TryParse(text, out value) && min <= value && value <= max;
        }

        public static bool IsIP(string text)
        {
            string[] IPparts = text.Split(new char[] { '.' }, 4);
            if (IPparts.Length < 4)
            {
                return false;
            }
            foreach (string part in IPparts) 
            {
                if(!IsIntBetween(part, 0, 255, out int temp))
                {
                    return false;
                }
            }
            return true;
        }

        public void SetIpPortInfo(string text)
        {
            ipPortInfo.text = text;
        }

        /// <summary>
        /// 获取指定的UI组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetOneUIComponent<T>(string panelName, string uiName) where T : UIBehaviour
        {
            Dictionary<string, List<UIBehaviour>> dic = allUI[panelName];
            if (dic.ContainsKey(uiName))
            {
                for (int i = 0; i < dic[uiName].Count; i++)
                {
                    if (dic[uiName][i] is T)
                    {
                        return (dic[uiName][i] as T);
                    }
                }
            }
            return null;
        }

        public void AddToMainDic(string panelName, Dictionary<string, List<UIBehaviour>> panelDic)
        {
            allUI.Add(panelName, panelDic);
        }
    }

}
