﻿using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyTopPanel : MonoBehaviour
    {
        readonly string connectionStatusMessage = "    Connection Status: ";

        [Header("UI References")]
        public Text ConnectionStatusText;

        #region UNITY

        public void Update()
        {
            ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
        }

        #endregion
    }
}