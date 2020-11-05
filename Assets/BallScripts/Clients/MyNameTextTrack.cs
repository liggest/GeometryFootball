using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallScripts.Clients
{
    public class MyNameTextTrack : MonoBehaviour
    {
        public Transform trackPlayer;

        private void Start()
        {
            GetComponent<Text>().text = Client.instance.myName;
        }
    }
}