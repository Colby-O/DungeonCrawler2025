using PlazmaGames.Core.Debugging;
using PlazmaGames.Runtime.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DC2025
{
	public class EntityData
	{
		public Entity entity;
		public Vector2Int loc;
    }

	public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
	{
		[SerializeField] private Vector2 _tileSize;

		[SerializeField] private SerializableDictionary<Vector2Int, Tile> _tiles;

		public List<EntityData> _entities;

		public Vector2 GetTileSize() => _tileSize;

        public List<EntityData> GetEntitesOnTile(Vector2Int pos)
        {
            return _entities.Where(e => e.loc == pos).ToList();
        }

        public void SetTileEnemySeen(Vector2Int pos)
        {
            if (_tiles.ContainsKey(pos))
            {
                _tiles[pos].SetEnemySeen();
            }
        }

        public void RemoveEntity(Entity entity)
        {
            int idx = _entities.FindIndex(e => e.entity == entity);
            if (idx >= 0) _entities.RemoveAt(idx);
        }

		public Vector2Int WorldToGrid(Vector3 worldPos)
		{
			Vector3 clamped = new Vector3(Mathf.Round(worldPos.x) / _tileSize.x, 0, Mathf.Round(worldPos.z) / _tileSize.y);
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

		public void RegisterEntity(Entity entity, Vector2Int loc)
		{
			EntityData data = new EntityData();
			data.entity = entity;
			data.loc = loc;

			if (entity is Player) UpdateTiles(data.loc, true);

            if (_entities != null) _entities.Add(data);
			else _entities = new List<EntityData> { data };
		}

		public void Sync(Entity entity, Vector2Int loc)
		{
			EntityData data = _entities.FirstOrDefault((e) => e.entity == entity);

			if (data == null)
			{
				RegisterEntity(entity, loc);
            }
			else
			{
                if (data.entity is Player)
                {
                    UpdateTiles(data.loc, false);
                    UpdateTiles(loc, true);
                }
                data.loc = loc;
            }
		}

		private void UpdateTiles(Vector2Int loc, bool onEnter = true)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					Vector2Int pos = new Vector2Int(loc.x + i, loc.y + j);
					if (_tiles.ContainsKey(pos))
					{
						if (i == 0 && j == 0)
						{
							_tiles[pos].OnPlayerEnterRequest = onEnter;
                        }
						else
						{
							_tiles[pos].OnPlayerAdjancentRequest = onEnter;
                        }
					}
				}
			}
		}

		private void RegisterTiles()
		{
			Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);

			foreach (Tile tile in tiles)
			{
				if (tile != null)
				{
					Vector2Int gridPos = WorldToGrid(tile.transform.position);
					if (!_tiles.ContainsKey(gridPos))
					{
                        _tiles.Add(gridPos, tile);
                    }
					else
					{
						PlazmaDebug.LogError($"Trying to Instinate {tile.name} at ({tile.transform.position.x}, {tile.transform.position.z}).", "ERROR", 1);
					}
				}
			}
		}

        private void RegisterInteractables()
		{
            MonoBehaviour[] objs = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            foreach (MonoBehaviour obj in objs)
            {
                if (obj != null && obj is IInteractable)
                {
                    Vector2Int gridPos = WorldToGrid(obj.transform.position);
                    if (_tiles.ContainsKey(gridPos))
                    {
						_tiles[gridPos].AddInteractable(obj as IInteractable);
                    }
                    else
                    {
                        PlazmaDebug.LogError($"Trying to Add Interacteable {obj.name} at ({obj.transform.position.x}, {obj.transform.position.z}) But There Is No Tile.", "ERROR", 1);
                    }
                }
            }
        }

        private void Awake()
        {
            _entities = new List<EntityData>();
        }

        private void Start()
		{
			RegisterTiles();
			RegisterInteractables();
        }

	}
}
