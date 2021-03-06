﻿using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System;
using BallScripts.Utils;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace BallScripts.GameLogics
{

    public class UITestScene : MonoBehaviour
    {

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

        }

        public void InitUI()
        {
            clientMenu.SetActive(true);
            serverMenu.SetActive(false);
            serverInfo.SetActive(false);

            serverPortField.interactable = true;
            clientPortField.interactable = true;
            ipField.interactable = true;
            usernameField.interactable = true;

        }

        public void ToServerMenu()
        {
            clientMenu.SetActive(false);
            serverMenu.SetActive(true);
        }

        public void ToClientMenu()
        {
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
    }

}
