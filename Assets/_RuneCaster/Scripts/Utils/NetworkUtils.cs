using System.Diagnostics.CodeAnalysis;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[SuppressMessage("ReSharper", "InvalidXmlDocComment")]
public class NetworkUtils : MonoBehaviour {
    void Awake() {
        // Register serialization custom types
        // RegisterType must be in Start (or earlier?), otherwise client issues
        PhotonPeer.RegisterType(typeof(PieceData), 255, PieceData.Serialize, PieceData.Deserialize);
        PhotonPeer.RegisterType(typeof(Block), 254, Block.Serialize, Block.Deserialize); 
        PhotonPeer.RegisterType(typeof(SpellProjectileData), 253, SpellProjectileData.Serialize, SpellProjectileData.Deserialize); 
    }

    /// <summary>
    /// RPC for basic transform changes. Can set parent of target object.
    /// </summary>
    /// <param name="targetID">PhotonView ID of object to change.</param>
    /// <param name="parentID">PhotonView ID of object to parent to. -1 = do not set parent</param>
    [PunRPC]
    public void S_SetTransform(int targetID, Vector3 position, Quaternion rotation, int parentID, bool keepWorldPosition) {
        Transform targetTransform = PhotonView.Find(targetID).gameObject.transform;
        
        // Disable any Photon sync components
        if (targetTransform.TryGetComponent(out PhotonTransformViewClassic ptvc)) {
            ptvc.enabled = false;
        }

        // Set parent if requested
        if (parentID != -1) {
            Transform parentTransform = PhotonView.Find(parentID).gameObject.transform;
            targetTransform.SetParent(parentTransform);
        }

        // Set transform
        if (!keepWorldPosition) {
            targetTransform.localPosition = position;
            targetTransform.localRotation = rotation;
        } else {
            targetTransform.position = position;
            targetTransform.rotation = rotation;
        }
    }

    /// <summary>
    /// RPC for resetting parent of target object.
    /// </summary>
    /// <param name="targetID">PhotonView ID of object to change.</param>
    /// <param name="parentID">PhotonView ID of object to parent to. -1 = do not set parent</param>
    [PunRPC]
    public void S_UnsetParent(int targetID) {
        Transform targetTransform = PhotonView.Find(targetID).gameObject.transform;
        
        // note: enable Photon sync components if needed
        
        targetTransform.SetParent(null);
    }

    [PunRPC]
    public void S_SetScale(int targetID, Vector3 scale) {
        Transform targetTransform = PhotonView.Find(targetID).gameObject.transform;
        targetTransform.localScale = scale;
    }
    
    // public static byte[] Vector2IntToBytes(Vector2Int vector) {
    //     using MemoryStream stream = new MemoryStream();
    //     using (BinaryWriter writer = new BinaryWriter(stream)) {
    //         writer.Write(vector.x);
    //         writer.Write(vector.y);
    //         return stream.ToArray();
    //     }
    // }
    //
    // public static Vector2Int BytesToVector2Int(byte[] bytes) {
    //     using MemoryStream stream = new MemoryStream(bytes);
    //     using (BinaryReader reader = new BinaryReader(stream)) {
    //         int x = reader.ReadInt32();
    //         int y = reader.ReadInt32();
    //         return new Vector2Int(x, y);
    //     }
    // }
}