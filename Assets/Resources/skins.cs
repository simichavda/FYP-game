using System.Collections.Generic;
using System;

[Serializable]
public class SkinData
{
	public string name;
	public int cost;
	public string iconPath;     // Path under Resources for the icon Sprite
	public string materialPath; // Path under Resources for the Material
}

[Serializable]
public class SkinDataList
{
	public List<SkinData> skins;
}
