using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// This used to have pooling, but is simplified for multiplayer logic
public class HeldPieceOutliner : MonoBehaviourPun {
    [SerializeField] Color _outlineColor;

    [SerializeField] GameObject _blockOutlinePrefab;

    // Move outlines on clients to fit held piece. Gets ref to block outline objs by heldPieceViewID since outline obj dont have one
    [PunRPC]
    public void S_CreateOutline(int heldPieceViewID) {
        Piece heldPiece = PhotonView.Find(heldPieceViewID).GetComponent<Piece>();
        if (heldPiece != null) {
            // Match block outlines to held piece shape
            List<GameObject> heldPieceBlockObjs = heldPiece.GetPieceBlockObjs();
            for (int i = 0; i < heldPiece.Shape.Count; i++) {
                GameObject blockObj = CreateBlockOutline();

                blockObj.transform.SetParent(heldPieceBlockObjs[i].transform);
                blockObj.transform.localPosition = Vector3.zero;
                blockObj.SetActive(true);
            }
        }
    }

    // Not efficient
    [PunRPC]
    public void S_DestroyOutline(int heldPieceViewID) {
        Piece heldPiece = PhotonView.Find(heldPieceViewID).GetComponent<Piece>();
        if (heldPiece != null) {
            // Match block outlines to held piece shape
            List<GameObject> heldPieceBlockObjs = heldPiece.GetPieceBlockObjs();
            for (int i = 0; i < heldPiece.Shape.Count; i++) {
                GameObject blockOutlineObj = heldPieceBlockObjs[i].transform.GetChild(0).gameObject;
                Destroy(blockOutlineObj);
            }
        }
    }

    GameObject CreateBlockOutline() {
        SpriteRenderer blockSr = Instantiate(_blockOutlinePrefab, transform).GetComponent<SpriteRenderer>();
        blockSr.color = _outlineColor;
        return blockSr.gameObject;
    }
}