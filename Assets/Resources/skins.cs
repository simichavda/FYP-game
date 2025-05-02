using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;

[Serializable]
public class SkinData
{
    public string DisplayName;
    public Sprite PreviewSprite;
	public Texture2D MeshTexture;
    public int Price;
    public SkinStatus Status; // Purchased, equipped, locked
}

[Serializable]
public enum SkinStatus
{
    Purchased,
    Equipped,
    Locked // Not purchased; can buy
}

[Serializable]
public class SkinDataList
{
	public List<SkinData> skins;
}
