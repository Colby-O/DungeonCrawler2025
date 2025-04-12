using PlazmaGames.Runtime.DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [SerializeField] private Vector2 _tileSize;

        [SerializeField] private SerializableDictionary<Vector2Int, Tile> _tiles;

        public Vector2 GetTileSize() => _tileSize;

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 clamped = new Vector3(worldPos.x / _tileSize.x, 0, worldPos.z / _tileSize.y);
            return new Vector2Int(Mathf.FloorToInt(clamped.x), Mathf.FloorToInt(clamped.z));
        }

        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * _tileSize.x, 0, gridPos.y * _tileSize.y);
        }

        public bool CanMoveTo(Vector2Int gridPos, Direction from)
        {
            if (_tiles.ContainsKey(gridPos)) return !_tiles[gridPos].HasWallAt(from);
            return false;
        }

        private void RegisterTiles()
        {
            Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);

            foreach (Tile tile in tiles)
            {
                if (tile != null)
                {
                    _tiles.Add(WorldToGrid(tile.transform.position), tile);
                }
            }
        }

        private void Start()
        {
            RegisterTiles();
        }

    }
}
