using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum PieceType {
    O1,
    // O2,
    // I2,
    // I3,
    I4,
    L,
}

public static class PieceTypeLookUp {
    public static Dictionary<PieceType, PieceData> LookUp = new Dictionary<PieceType, PieceData>() {
        {
            PieceType.O1, new PieceData() {
                Shape = new List<Vector2Int>() {
                    new(0,0)
                }
            }
        }, {
            PieceType.L, new PieceData() {
                Shape = new List<Vector2Int>() {
                    new(0,0),
                    new(0,1),
                    new(0,-1),
                    new(1,-1),
                }
            }
        }, {
            PieceType.I4, new PieceData() {
                Shape = new List<Vector2Int>() {
                    new(0,0),
                    new(0,1),
                    new(0,-1),
                    new(0,-2),
                }
            }
        },
    };
}

[Serializable]
public struct PieceData {
    public List<Vector2Int> Shape;
    public Color Color;
    public bool CanRotate;

    #region Serialization

    public static byte[] Serialize(object input) {
        var data = (PieceData) input;
        
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream)) {
            writer.Write(data.Shape.Count);
            foreach (Vector2Int pos in data.Shape) {
                writer.Write(pos.x);
                writer.Write(pos.y);
            }

            writer.Write(data.Color.r);
            writer.Write(data.Color.g);
            writer.Write(data.Color.b);
            writer.Write(data.Color.a);

            writer.Write(data.CanRotate);

            return stream.ToArray();
        }
    }

    public static object Deserialize(byte[] data) {
        PieceData result = new PieceData();

        using (MemoryStream stream = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(stream)) {
            int shapeCount = reader.ReadInt32();
            List<Vector2Int> shape = new List<Vector2Int>();
            for (int i = 0; i < shapeCount; i++) {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                shape.Add(new Vector2Int(x, y));
            }
            result.Shape = shape;

            float r = reader.ReadSingle();
            float g = reader.ReadSingle();
            float b = reader.ReadSingle();
            float a = reader.ReadSingle();
            result.Color = new Color(r, g, b, a);

            result.CanRotate = reader.ReadBoolean();
        }

        return result;
    }

    #endregion
}