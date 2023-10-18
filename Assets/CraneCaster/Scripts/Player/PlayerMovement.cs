using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviourPun {
    [SerializeField] float _maxSpeed = 10f;
    [SerializeField] float _maxAcceleration = 10f;

    public Vector2 moveInput;

    Rigidbody2D rb;

    public Action<float> OnUpdateSpeedDuration;

    void Awake() {
        _origMaxSpeed = _maxSpeed;
        _origMaxAcceleration = _maxAcceleration;
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 dir = new Vector2(moveInput.x, moveInput.y);
        dir = Vector2.ClampMagnitude(dir, 1f);

        Vector2 targetV = dir * _maxSpeed;

        AccelerateTo(targetV);
    }

    void AccelerateTo(Vector2 targetVelocity) {
        float limit = _maxAcceleration;

        Vector2 deltaV = targetVelocity - rb.velocity;

        Vector2 accel = deltaV / Time.fixedDeltaTime;
        accel = Vector2.ClampMagnitude(accel, limit);

        rb.AddForce(accel, ForceMode2D.Force);
    }

    public void MultiplySpeedForDuration(float percent, float duration) {
        if (_multiplySpeedTimer != null) {
            StopCoroutine(_multiplySpeedTimer);
            _maxSpeed = _origMaxSpeed;
            _maxAcceleration = _origMaxAcceleration;
        }
        _multiplySpeedTimer = StartCoroutine(MultiplySpeedTimer(percent, duration));
    }
    
    float _origMaxSpeed;
    float _origMaxAcceleration;
    Coroutine _multiplySpeedTimer;
    IEnumerator MultiplySpeedTimer(float percent, float duration) {
        _maxSpeed *= percent;
        _maxAcceleration *= percent;

        float timer = duration;
        while (timer > 0) {
            timer -= Time.deltaTime;
            OnUpdateSpeedDuration.Invoke(timer / duration);

            yield return null;
        }

        _maxSpeed = _origMaxSpeed;
        _maxAcceleration = _origMaxAcceleration;
    }
}