using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_UnitData : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		UnitData data = (UnitData)obj;
		// Add your writer.Write calls here.
		writer.Write(data.characterName);
		writer.Write(data.unitPrefab);
		writer.Write(data.level);
		writer.Write(data.xp);

	}
	
	public override object Read(ES2Reader reader)
	{
		UnitData data = new UnitData();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		UnitData data = (UnitData)c;
		// Add your reader.Read calls here to read the data into the object.
		data.characterName = reader.Read<System.String>();
		data.unitPrefab = reader.Read<System.String>();
		data.level = reader.Read<System.Int32>();
		data.xp = reader.Read<System.Int32>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_UnitData():base(typeof(UnitData)){}
}