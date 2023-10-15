using System.IO;
using UnityEngine;

public enum SpellType {
	
}

public struct SpellData {
	public int Dmg;
	public float Speed;
	public Vector2 MoveDir;
	
	#region Serialization

	public static byte[] Serialize(object input) {
		var data = (SpellData) input;
        
		using (MemoryStream stream = new MemoryStream())
		using (BinaryWriter writer = new BinaryWriter(stream)) {
			writer.Write(data.Dmg);
			writer.Write(data.Speed);
			writer.Write(data.MoveDir.x);
			writer.Write(data.MoveDir.y);

			return stream.ToArray();
		}
	}

	public static object Deserialize(byte[] data) {
		SpellData result = new SpellData();

		using (MemoryStream stream = new MemoryStream(data))
		using (BinaryReader reader = new BinaryReader(stream)) {
			result.Dmg = reader.ReadInt32();
			result.Speed = reader.ReadSingle();
			Vector2 moveDir = new Vector2();
			moveDir.x = reader.ReadSingle();
			moveDir.y = reader.ReadSingle();
			result.MoveDir = moveDir;
		}

		return result;
	}

	#endregion
}
