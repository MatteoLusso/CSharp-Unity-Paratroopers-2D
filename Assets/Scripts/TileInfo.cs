using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class TileInfo
{
    public Vector2 tile_PositionInMatrix;
    public EnumTileInfo.MaxSpeed allowedMovement;
    //public int city_Block;

    public TileInfo(int x, int y)
    {
        tile_PositionInMatrix = new Vector2(x, y);
        //city_Block = -1;
    }

    public static class EnumTileInfo
    {
        public enum MaxSpeed{Slow = 1, Normal = 2, Fast = 3};

    }
}
