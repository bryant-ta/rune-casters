using System;
using System.Collections;
using Photon.Pun;
using Timers;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviourPun {
    public float MaxSpeed => _maxSpeed;
    [SerializeField] float _maxSpeed = 10f;
    public float MaxAcceleration => _maxAcceleration;
    [SerializeField] float _maxAcceleration = 80f;

    public Vector2 moveInput;

    Rigidbody2D rb;

    // Speeding Spell
    public CountdownTimer SpeedingTimer;
    float _origMaxSpeed;
    float _origMaxAcceleration;

    void Awake() {
        _origMaxSpeed = _maxSpeed;
        _origMaxAcceleration = _maxAcceleration;

        SpeedingTimer = new CountdownTimer(Constants.SpellDuration);
        SpeedingTimer.EndEvent += ResetSpeed;
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

    public void AddVelocity(Vector2 velocity) {
        rb.velocity += velocity;
    }
    public void SetVelocity(Vector2 velocity) {
        rb.velocity = velocity;
    }
    public float GetSpeed() {
        return rb.velocity.magnitude;
    }

    public void MultiplySpeedForDuration(float percent) {
        if (SpeedingTimer.IsTicking) {
            _maxSpeed = _origMaxSpeed;
            _maxAcceleration = _origMaxAcceleration;
        }
        
        _maxSpeed *= percent;
        _maxAcceleration *= percent;
        
        SpeedingTimer.Start();
    }
    void ResetSpeed() {
        _maxSpeed = _origMaxSpeed;
        _maxAcceleration = _origMaxAcceleration;
    }
}