using Photon.Pun;
using UnityEngine;

public class Factory : MonoBehaviourPun {
    public static Factory Instance { get; private set; }

    public static GameObject PieceBase { get; private set; }
    [SerializeField] GameObject _pieceBase;
    public static GameObject BlockBase { get; private set; }
    [SerializeField] GameObject _blockBase;
    
    public static GameObject SpellBase { get; private set; }
    [SerializeField] GameObject _spellBase;
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        PieceBase = _pieceBase;
        BlockBase = _blockBase;
        SpellBase = _spellBase;
    }

    public Piece CreatePieceObj(PieceData pieceData, Vector2 position) {
        object[] initData = {pieceData};
        GameObject pieceObj = PhotonNetwork.Instantiate(Constants.PhotonPrefabsPath + _pieceBase.name, position,
            _pieceBase.transform.rotation, 0, initData);

        return pieceObj.GetComponent<Piece>();
    }

    public Block CreateBlockObj(Block block, Vector2 position) {
        object[] initData = {block};
        GameObject blockObj = PhotonNetwork.Instantiate(Constants.PhotonPrefabsPath + _blockBase.name, position,
            _blockBase.transform.rotation, 0, initData);

        return blockObj.GetComponent<Block>();
    }
    
    // Currently, spells exist and move locally on each client but only master registers hits and damage calculations, then rpcs display effects
    [PunRPC]
    public SpellProjectile S_CreateSpellObj(SpellProjectileData spellProjectileData, Vector2 position, PhotonMessageInfo info) {
        SpellProjectile spellProjectile = Instantiate(_spellBase, position, Quaternion.identity).GetComponent<SpellProjectile>();
        
        float lag = (float) (PhotonNetwork.Time - info.SentServerTime);
        spellProjectile.Init(spellProjectileData, lag);

        return spellProjectile;
    }
}