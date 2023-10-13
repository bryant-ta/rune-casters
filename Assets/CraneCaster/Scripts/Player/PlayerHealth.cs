using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerUI))]
public class PlayerHealth : MonoBehaviourPunCallbacks {
    public int Hp => _hp;
    [SerializeField] int _hp;
    public int MaxHp => _maxHp;
    [SerializeField] int _maxHp;

    PlayerUI _playerUI;

    void Awake() { _playerUI = GetComponent<PlayerUI>(); }

    /// <summary>
    /// Changes player health.
    /// </summary>
    /// <param name="value">Positive input heals. Negative input damages.</param>
    /// <returns>Actual health value delta including sign</returns>
    public int ModifyHp(int value) {
        int newHp = _hp + value;
        if (newHp <= 0) {
            newHp = 0;
        } else if (newHp > _maxHp) {
            newHp = _maxHp;
        }

        int delta = newHp - _hp;
        _hp = newHp;
        Hashtable properties = new Hashtable() {{CustomPropertiesLookUp.LookUp[CustomPropertiesKey.Hp], _hp}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        
        // Health display updated per client in OnPlayerPropertiesUpdate, TODO: consider updating actual health value there too?

        // Death
        if (_hp == 0) {
            Debug.Log("Player died");
        }

        return delta;
    }

    // Called on other clients after local client calculates final changed hp
    // note: OnPlayerPropertiesUpdate gets called on local client too, should only use SetCustomProperties new value after this callback to be synced
    // note: every instance of Player will trigger this callback, need to modify correct player form GameManager.PlayerList
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) {
        // Ensures anything below only runs on Player that sent the property update (on each client)
        if (GetComponent<Player>().PlayerId != targetPlayer.ActorNumber) return;
        
        GameManager.Instance.PlayerList[targetPlayer.ActorNumber].GetComponent<PlayerUI>().UpdateHpBar((float) (int) changedProps[CustomPropertiesLookUp.LookUp[CustomPropertiesKey.Hp]] / _maxHp);
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (!PhotonNetwork.IsMasterClient) return;

        if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Spell])) {
            print("collided spell");
            Spell spell = col.gameObject.GetComponent<Spell>();
            ModifyHp(-spell.Dmg);
        }
    }
}