using System;
using System.IO;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class Block {
    public Vector2Int Position;
    public SpellType SpellType;
    public bool IsActive;

    public Block(Vector2Int position, SpellType spellType = SpellType.None, bool isActive = true) {
        Position = position;
        SpellType = spellType;
        IsActive = isActive;
    }

    public void MoveTo(Vector2Int pos) {
        Position = new Vector2Int(pos.x, pos.y);
    }

    public void MoveTo(int x, int y) {
        Position = new Vector2Int(x, y);
    }

    #region Serialization
    
    public static byte[] Serialize(object input)
    {
    	var data = (Block)input;
       
    	// Create a MemoryStream to store the serialized data
    	using (MemoryStream stream = new MemoryStream())
    	using (BinaryWriter writer = new BinaryWriter(stream))
    	{
    		// Serialize each field of the Block class
    		writer.Write(data.Position.x);
    		writer.Write(data.Position.y);
            writer.Write((int)data.SpellType);
    		writer.Write(data.IsActive);
           
    		// Convert the MemoryStream to a byte array and return it
    		return stream.ToArray();
    	}
    }
    
    public static object Deserialize(byte[] data)
    {
    	// Create a Block object with default values
    	var result = new Block(Vector2Int.zero);
       
    	// Create a MemoryStream from the input data
    	using (MemoryStream stream = new MemoryStream(data))
    	using (BinaryReader reader = new BinaryReader(stream))
    	{
    		// Deserialize each field of the Block class
    		result.Position.x = reader.ReadInt32();
    		result.Position.y = reader.ReadInt32();
            result.SpellType = (SpellType) reader.ReadInt32();
    		result.IsActive = reader.ReadBoolean();
    	}
       
    	return result;
    }
    
    #endregion
}