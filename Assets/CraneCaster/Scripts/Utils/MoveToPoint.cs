using System;
using Photon.Pun;
using UnityEngine;

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

        if (PhotonNetwork.IsMasterClient) {
            if (Vector3.Distance(transform.position, _endPos) < 0.001f) {
                OnReachedEnd.Invoke(gameObject);
                return;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, _endPos, MoveSpeed * Time.deltaTime);
    }

    public void SetMoveToPoint(Vector2 point) {
        _endPos = point;
    }

    [PunRPC]
    public void Disable() {
        enabled = false;
    }
}