using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class Board : MonoBehaviourPun {
    public Block[,] Blocks => _blocks;
    Block[,] _blocks; // TODO: write editor script to only show during play

    public int Width => _width;
    public int Height => _height;
    [SerializeField] int _width, _height;

    BoardRenderer _br;

    void Start() {
        Init(_width, _height);
    }

    public void Init(int width, int height) {
        _width = width;
        _height = height;
        
        _blocks = new Block[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                _blocks[x, y] = new Block(new Vector2Int(x, y), Color.black, false);
            }
        }
        
        // Init BoardRenderer with populated Block list
        if (TryGetComponent(out BoardRenderer br)) {
            _br = br;
            _br.Init(this);
        }
    }

    public bool PlacePiece(Piece piece, Vector2Int hoverPos) {
        // Debug.Log("PlaceBlock - hoverPos: " + hoverPos);

        // width and height /2 when board center is gameobject origin
        // Vector2Int pieceOrigin = new Vector2Int(hoverPos.x + _width/2, hoverPos.y + _height/2);
        Vector2Int pieceOrigin = new Vector2Int(hoverPos.x, hoverPos.y); // board bottom left corner is gameobject origin
        List<Block> newBlocks = new();

        // Validate placed locations
        foreach (Block block in piece.Blocks) {
            Vector2Int boardPos = new Vector2Int(pieceOrigin.x + block.Position.x, pieceOrigin.y + block.Position.y);

            if (!IsValidPlacement(boardPos.x, boardPos.y)) return false;
            
            newBlocks.Add(block);
            // Debug.Log("Point: " + boardPos);
        }

        // Passed placement checks, update board with new block
        foreach (Block newBlock in newBlocks) {
            photonView.RPC(nameof(S_UpdateBlock), RpcTarget.All, newBlock, pieceOrigin.x, pieceOrigin.y);
        }

        // Cleanup placed piece
        PhotonNetwork.Destroy(piece.gameObject);
        
        return true;
    }

    public int CalculateBoardDamage() {
        // TODO: implement board power scoring based on matching color groups and size
        return 10;
    }

    #region Helper

    [PunRPC]
    public void S_UpdateBlock(Block newBlock, int originX, int originY) {
        Vector2Int boardPos = new Vector2Int(originX + newBlock.Position.x, originY + newBlock.Position.y);

        Block boardBlock = _blocks[boardPos.x, boardPos.y];
        boardBlock.IsActive = true;
        boardBlock.Color = newBlock.Color;

        _br.Render();
    }

    public bool IsValidPlacement(int x, int y) { return IsInBounds(x, y) && !_blocks[x,y].IsActive; }
    public bool IsInBounds(int x, int y) { return !(x < 0 || x >= _width || y < 0 || y >= _height); }
    
    #endregion
}