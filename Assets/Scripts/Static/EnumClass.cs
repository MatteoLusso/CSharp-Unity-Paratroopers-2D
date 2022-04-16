using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enum
    {
        public enum TileType{Plain, Mountain, Forest, Lake, Road, City, River}
        public enum Size{Small, Medium, Big}
        public enum ErosionForce{None, Low, Strong}
        public enum Cardinal{None = -1, East, SouthEast, South, SouthWest, West, NorthWest, North, NorthEast};
        public enum Player{Empty, P1, P2, P3, P4};
    }
