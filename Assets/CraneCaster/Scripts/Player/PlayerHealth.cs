using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerHealth : MonoBehaviourPunCallbacks {
    public int Hp => _hp;
    [SerializeField] int _hp;
    public int MaxHp => _maxHp;
    [SerializeField] int _maxHp;

    Player _player;

    public Action<float> OnUpdateHp;

    void Awake() {
        _player = GetComponent<Player>();
    }

    /// <summary>
    /// Changes player health.
    /// </summary>
    /// <param name="value">Positive input heals. Negative input damages.</param>
    /// <returns>Actual health value delta including sign</returns>
    public int ModifyHp(int value) {
        if (!PhotonNetwork.IsMasterClient) return 0; // Only master should modify Player hp
        
        int newHp = _hp + value;
        if (newHp <= 0) {
            newHp = 0;
        } else if (newHp > _maxHp) {
            newHp = _maxHp;
        }

        Hashtable properties = new Hashtable() {{CustomPropertiesLookUp.LookUp[CustomPropertiesKey.Hp], newHp}};
        PhotonNetwork.PlayerList[_player.PlayerId - 1].SetCustomProperties(properties);
        // Health updated and display updated per client in OnPlayerPropertiesUpdate.

        // Death
        if (_hp == 0) {
            Debug.Log("Player died");
            GameManager.Instance.photonView.RPC(nameof(GameManager.DisablePlayerObj), RpcTarget.AllBuffered, _player.PlayerId);
        }

        return newHp - _hp;
    }

    // Called on all clients on updating player after master calculates final changed hp
    // note: OnPlayerPropertiesUpdate gets called on local client too, should only use SetCustomProperties' new value after this callback to be synced
    // note: every instance of Player will trigger this callback, need to apply to correct player from GameManager.PlayerList
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) {
        // Ensures below only runs on Player instance that sent the property update (still runs on each client)
        if (_player.PlayerId != targetPlayer.ActorNumber) return;
        Debug.Log($"Properties origin: {_player.PlayerId}");
        
        _hp = (int) changedProps[CustomPropertiesLookUp.LookUp[CustomPropertiesKey.Hp]];
        // _playerUI.UpdateHpBar((float) _hp / _maxHp);
        OnUpdateHp.Invoke((float) _hp / _maxHp);
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (!PhotonNetwork.IsMasterClient) return;

        if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Spell])) {
            // Debug.Log($"Spell hit Player {_player.PlayerId}");
            Spell spell = col.gameObject.GetComponent<Spell>();
            ModifyHp(-spell.Dmg);
        }
    }
}