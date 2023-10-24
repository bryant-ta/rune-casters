using System;
using ExitGames.Client.Photon;
using Game;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerHealth : MonoBehaviourPunCallbacks {
    public int Hp => _hp;
    [SerializeField] int _hp;
    public int MaxHp => _maxHp;
    [SerializeField] int _maxHp;
    
    public int Shield => _shield;
    [SerializeField] int _shield;
    public int MaxShield => _maxShield;
    [SerializeField] int _maxShield;

    Player _player;

    public Action<float> OnUpdateHp;
    public Action OnLoseHp;
    public Action<float> OnUpdateShield;

    void Awake() {
        _player = GetComponent<Player>();
    }

    /// <summary>
    /// Changes player HP.
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

        Hashtable properties = new Hashtable() {{CustomPropertiesKey.Hp, newHp}};
        PhotonNetwork.PlayerList[_player.PlayerId - 1].SetCustomProperties(properties);
        // Health updated and display updated per client in OnPlayerPropertiesUpdate.

        // Death
        if (newHp == 0) {
            Debug.Log("Player died");
            NetworkManager.Instance.photonView.RPC(nameof(NetworkManager.DisablePlayerObj), RpcTarget.AllBuffered, _player.PlayerId);
        }

        return newHp - _hp;
    }
    
    /// <summary>
    /// Changes player shield amount.
    /// </summary>
    /// <param name="value">Positive input adds shield. Negative input removes shield.</param>
    /// <returns>Actual shield value delta including sign</returns>
    public int ModifyShield(int value) {
        if (!PhotonNetwork.IsMasterClient) return 0; // Only master should modify
        
        int newShield = _shield + value;
        if (newShield <= 0) {
            newShield = 0;
        } else if (newShield > _maxShield) {
            newShield = _maxShield;
        }

        Hashtable properties = new Hashtable() {{CustomPropertiesKey.Shield, newShield}};
        PhotonNetwork.PlayerList[_player.PlayerId - 1].SetCustomProperties(properties);
        // Value updated and display updated per client in OnPlayerPropertiesUpdate.

        // Shield broken
        int delta = newShield - _shield;
        if (newShield == 0) {
            ModifyHp(value - delta);
        }

        return delta;
    }

    // Called on all clients on updating player after master calculates final changed hp
    // note: OnPlayerPropertiesUpdate gets called on local client too, should only use SetCustomProperties' new value after this callback to be synced
    // note: every instance of Player will trigger this callback, need to apply to correct player from GameManager.PlayerList
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) {
        // Ensures below only runs on Player instance that sent the property update (still runs on each client)
        if (_player.PlayerId != targetPlayer.ActorNumber) return;

        if (changedProps[CustomPropertiesKey.Hp] != null) {
            int oldHp = _hp;
            _hp = (int) changedProps[CustomPropertiesKey.Hp];
            OnUpdateHp.Invoke((float) _hp / _maxHp);
            
            // Damage to Player causes dropping any held piece
            if (oldHp > _hp) {
                OnLoseHp?.Invoke();
            }
        }
        if (changedProps[CustomPropertiesKey.Shield] != null) {
            _shield = (int) changedProps[CustomPropertiesKey.Shield];
            OnUpdateShield.Invoke((float) _shield / _maxShield);
        }
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (!PhotonNetwork.IsMasterClient) return;

        if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Spell])) {
            SpellProjectile spellProjectile = col.gameObject.GetComponent<SpellProjectile>();
            ModifyHp(-spellProjectile.Dmg);
        }
        if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Punch])) {
            PunchHitbox punchHitbox = col.gameObject.GetComponent<PunchHitbox>();
            ModifyHp(-punchHitbox.Player.PunchDmg);
            _player.StunnedTimer.Start();
        }
    }
}