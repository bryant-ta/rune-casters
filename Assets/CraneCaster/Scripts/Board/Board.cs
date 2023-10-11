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

    void Start() { Init(_width, _height); }

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

    public bool PlacePiece(Piece piece, int hoverX, int hoverY) {
        // Debug.Log("PlaceBlock - hoverPos: " + hoverPos);

        Vector2Int pieceOrigin = new Vector2Int(hoverX, hoverY); // board bottom left corner is gameobject origin
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

    // Returns group of adjacent blocks with the same color
    public int CountBlockGroup(int originX, int originY) {
        if (!IsInBounds(originX, originY)) return 0;
        return CountAdjacentColor(originX, originY, _blocks[originX, originY].Color, new bool[_width,_height]);
    }

    // Recursively find adjacent blocks of the same color as origin
    int CountAdjacentColor(int x, int y, Color originColor, bool[,] visited) {
        // Skip if block is out of bounds, already counted, is inactive(empty), or not the same color as origin
        if (!IsInBounds(x, y) || visited[x, y] || !_blocks[x, y].IsActive || _blocks[x, y].Color != originColor) return 0;

        visited[x, y] = true;
        int count = 1;
        Vector2Int[] directions = {new (1,0), new (0,1), new (-1,0), new (0,-1)};

        foreach (Vector2Int dir in directions) {
            int newX = x + dir.x;
            int newY = y + dir.y;
            count += CountAdjacentColor(newX, newY, originColor, visited);
        }

        return count;
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

    public Block SelectRelative(Vector2Int origin, Vector2Int offset) {
        int targetX = origin.x + offset.x;
        int targetY = origin.y + offset.y;

        if (!IsInBounds(targetX, targetY)) return null;

        return _blocks[targetX, targetY];
    }

    public bool IsValidPlacement(int x, int y) { return IsInBounds(x, y) && !_blocks[x, y].IsActive; }
    public bool IsInBounds(int x, int y) { return !(x < 0 || x >= _width || y < 0 || y >= _height); }

    #endregion
}