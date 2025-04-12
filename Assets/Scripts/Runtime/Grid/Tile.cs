using System;
using System.Collections.Generic;
using UnityEngine;
using PlazmaGames.Runtime.DataStructures;

namespace DC2025
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<Direction, bool> _walls;

        public bool HasWallAt(Direction dir)
        {
            dir = dir.GetFacingDirection(-transform.rotation.eulerAngles.y);
            if (_walls.ContainsKey(dir)) return _walls[dir];
            return false;
        }
    }
}
