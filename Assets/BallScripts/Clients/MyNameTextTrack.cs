using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallScripts.Clients
{
    public class MyNameTextTrack : MonoBehaviour
    {
        public Transform trackPlayer;
        private Vector3 playerScreenPos;
        private Text myPlayerNameText;

        private void Start()
        {
            myPlayerNameText = GetComponent<Text>();
            myPlayerNameText.text = Client.instance.myName;
        }

        private void FixedUpdate()
        {
            playerScreenPos = Camera.main.WorldToScreenPoint(new Vector3(trackPlayer.position.x, trackPlayer.position.y, trackPlayer.position.z));
            myPlayerNameText.rectTransform.position = new Vector3(playerScreenPos.x, playerScreenPos.y + 90, 0);
        }
    }
}