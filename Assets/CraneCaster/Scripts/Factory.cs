using Photon.Pun;
using UnityEngine;

public class Factory : MonoBehaviour {
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

    bool _flag = true;
    void Update() {
        // DEBUG
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom && _flag) {
            PieceData bd = PieceTypeLookUp.LookUp[PieceType.L];
            bd.Color = Color.cyan;
            bd.CanRotate = true;

            Piece p = CreatePieceObj(bd, new Vector2(0, 2));
            p.photonView.RPC(nameof(MoveToPoint.Disable), RpcTarget.All);

            _flag = false;
        }
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
    
    public Spell CreateSpellObj(SpellData spellData, Vector2 position) {
        object[] initData = {spellData};
        GameObject spellObj = PhotonNetwork.Instantiate(Constants.PhotonPrefabsPath + _spellBase.name, position,
            _spellBase.transform.rotation, 0, initData);
        
        Spell spell = spellObj.GetComponent<Spell>();
        spell.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);

        return spell;
    }
}