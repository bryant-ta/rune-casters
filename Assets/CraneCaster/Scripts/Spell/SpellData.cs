using System.IO;

public enum SpellType {
	
}

public struct SpellData {
	public int Dmg;
	public float Speed;
	
	#region Serialization

	public static byte[] Serialize(object input) {
		var data = (SpellData) input;
        
		using (MemoryStream stream = new MemoryStream())
		using (BinaryWriter writer = new BinaryWriter(stream)) {
			writer.Write(data.Dmg);
			writer.Write(data.Speed);

			return stream.ToArray();
		}
	}

	public static object Deserialize(byte[] data) {
		SpellData result = new SpellData();

		using (MemoryStream stream = new MemoryStream(data))
		using (BinaryReader reader = new BinaryReader(stream)) {
			int dmg = reader.ReadInt32();
			result.Dmg = dmg;
			float speed = reader.ReadSingle();
			result.Speed = speed;
		}

		return result;
	}

	#endregion
}
