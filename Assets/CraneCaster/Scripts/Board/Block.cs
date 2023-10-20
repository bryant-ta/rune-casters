using System;
using System.IO;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class Block {
    public Vector2Int Position;
    public Color Color;
    public bool IsActive;

    public Block(Vector2Int position, Color color, bool isActive = true) {
        Position = position;
        Color = color;
        IsActive = isActive;
    }

    public void MoveTo(Vector2Int pos) {
        // TODO: add event triggering render in BlockRenderer
        Position = new Vector2Int(pos.x, pos.y);
    }

    public void MoveTo(int x, int y) {
        // TODO: add event triggering render in BlockRenderer
        Position = new Vector2Int(x, y);
    }

    #region Serialization
    
    public static byte[] Serialize(object input)
    {
    	var block = (Block)input;
       
    	// Create a MemoryStream to store the serialized data
    	using (MemoryStream stream = new MemoryStream())
    	using (BinaryWriter writer = new BinaryWriter(stream))
    	{
    		// Serialize each field of the Block class
    		writer.Write(block.Position.x);
    		writer.Write(block.Position.y);
    		writer.Write(block.Color.r);
    		writer.Write(block.Color.g);
    		writer.Write(block.Color.b);
    		writer.Write(block.Color.a);
    		writer.Write(block.IsActive);
           
    		// Convert the MemoryStream to a byte array and return it
    		return stream.ToArray();
    	}
    }
    
    public static object Deserialize(byte[] data)
    {
    	// Create a Block object with default values
    	var result = new Block(Vector2Int.zero, Color.white);
       
    	// Create a MemoryStream from the input data
    	using (MemoryStream stream = new MemoryStream(data))
    	using (BinaryReader reader = new BinaryReader(stream))
    	{
    		// Deserialize each field of the Block class
    		result.Position.x = reader.ReadInt32();
    		result.Position.y = reader.ReadInt32();
    		result.Color.r = reader.ReadSingle();
    		result.Color.g = reader.ReadSingle();
    		result.Color.b = reader.ReadSingle();
    		result.Color.a = reader.ReadSingle();
    		result.IsActive = reader.ReadBoolean();
    	}
       
    	return result;
    }
    
    #endregion
}