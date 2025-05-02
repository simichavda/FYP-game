using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SkinData
{
    public string DisplayName;
    public Sprite PreviewSprite;
	public Texture2D MeshTexture;
    public int Price;
}

[Serializable]
public class SkinDataList
{
	public List<SkinData> skins;
}
