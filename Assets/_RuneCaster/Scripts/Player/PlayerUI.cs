using System;
using Game;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player), typeof(PlayerHealth))]
public class PlayerUI : MonoBehaviour, IPunObservable {
    [SerializeField] Image _hpBar;
    [SerializeField] Image _shieldBar;
    [SerializeField] Image _speedbar;
    [SerializeField] Image _spellLifespanBar;

    Player _player;
    PlayerHealth _playerHealth;
    PlayerMovement _playerMovement;

    void Awake() {
        _player = GetComponent<Player>();
        _playerHealth = GetComponent<PlayerHealth>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    void Start() {
        _playerHealth.OnUpdateHp += UpdateHpBar;
        _playerHealth.OnUpdateShield += UpdateShieldBar;
        _playerMovement.SpeedingTimer.TickEvent += UpdateSpeedBar;
        _player.SpellUseableTimer.TickEvent += UpdateSpellLifespanBar;
    }

    void UpdateHpBar(float percent) { _hpBar.fillAmount = percent; }

    void UpdateShieldBar(float percent) {
        //DEBUG
        return;
        _shieldBar.fillAmount = percent;
    }
    
    void UpdateSpeedBar(float percent) {
        _speedbar.fillAmount = percent;
    }

    void UpdateSpellLifespanBar(float percent) { _spellLifespanBar.fillAmount = percent; }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            // We own this player: send the others our data
            stream.SendNext(_speedbar.fillAmount);
            stream.SendNext(_spellLifespanBar.fillAmount);
        } else {
            // Network player, receive data
            _speedbar.fillAmount = (float) stream.ReceiveNext();
            _spellLifespanBar.fillAmount = (float) stream.ReceiveNext();
        }
    }
}