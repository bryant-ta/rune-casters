using System;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Move attached GameObject to target point. Should only be enabled on master since clients receive transform from PhotonTransformView
/// Enable component to start movement. Disable to stop movement.
/// </summary>
[RequireComponent(typeof(PhotonTransformViewClassic))]
public class MoveToPoint : MonoBehaviour {
    public float MoveSpeed;
    public Transform EndPoint;

    Vector2 _endPos;

    public Action<GameObject> OnReachedEnd;

    void Awake() {
        if (EndPoint) _endPos = EndPoint.position;

        if (!PhotonNetwork.IsMasterClient) enabled = false;
    }

    // set enabled = false to stop
    void Update() {
        if (!enabled) return;
        if (!PhotonNetwork.IsMasterClient) {
            enabled = false;
            return;
        }

        // Only master runs below
        if (Vector3.Distance(transform.position, _endPos) < 0.001f) {
            enabled = false;
            OnReachedEnd.Invoke(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _endPos, MoveSpeed * Time.deltaTime);
    }

    // Useful when setting point as just a position instead of using an existing Transform
    public void SetMoveToPoint(Vector2 point) {
        _endPos = point;
    }

    [PunRPC]
    public void Disable() {
        enabled = false;
    }
}