using PlazmaGames.Core.Debugging;
using PlazmaGames.Runtime.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Core;
using PlazmaGames.Core.Utils;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UIElements;

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
        private Vector2Int _lastPlayerPos = new Vector2Int(-100000, -100000);

        public Vector2 GetTileSize() => _tileSize;

		public Tile GetTileAt(int x, int y) => GetTileAt(new Vector2Int(x, y));

		public Tile GetTileAt(Vector2Int gridPos) => _tiles[gridPos];

		public (Vector2Int, Tile) FindVaildLocationNearPlayer()
		{
			Queue<Vector2Int> possibleLocations = new Queue<Vector2Int>();
			Dictionary<Vector2Int, bool> visted = new Dictionary<Vector2Int, bool>();

			Vector2Int playerPos = WorldToGrid(DCGameManager.Player.transform.position);
			possibleLocations.Enqueue(playerPos);
			visted[playerPos] = true;

			(Vector2Int, Tile) res = (Vector2Int.zero, null);

			while (possibleLocations.Count > 0) 
			{
				Vector2Int pos = possibleLocations.Dequeue();
				if (_tiles.ContainsKey(pos) && !_tiles[pos].HasInteractable())
				{
					res = (pos, GetTileAt(pos));
					break;
				}
				else
				{
					for (int i = -1; i <= 1; i++)
					{
						for (int j = -1; j <= 1; j++)
						{
							Vector2Int nextPos = pos + new Vector2Int(i, j);

							if (!visted.ContainsKey(nextPos) || !visted[nextPos])
							{
								possibleLocations.Enqueue(nextPos);
								visted.Add(nextPos, true);
							}
						}
					}
				}
			}

			return res;
		}

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

        public void UnsetTileDistraction(Vector2Int pos)
        {
			if (_tiles.ContainsKey(pos))
			{
				_tiles[pos].UnsetDistraction();
			}
        }
		public void SetTileDistraction(Vector2Int pos)
		{
			if (_tiles.ContainsKey(pos))
			{
				_tiles[pos].SetDistraction();
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

		public bool CanMoveTo(Vector2Int gridPos, Direction dir, bool forceDoorsOpen = false, bool ignoreDoors = false)
		{
            if (_tiles.ContainsKey(gridPos))
            {
                return (
                    !_tiles[gridPos].HasWallAt(dir, ignoreDoors, forceDoorsOpen, ignoreDoors) &&
                    (_tiles.ContainsKey(gridPos + dir.ToVector2Int()) && !_tiles[gridPos + dir.ToVector2Int()].HasWallAt(dir.Opposite(), ignoreDoors, forceDoorsOpen, ignoreDoors))
                );
            }
			return false;
		}

		public void RegisterEntity(Entity entity, Vector2Int loc)
		{
			EntityData data = new EntityData();
			data.entity = entity;
			data.loc = loc;

			if (entity is Player) UpdateTiles(data.loc, data.entity as Player, true);

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
					UpdateTiles(data.loc, data.entity as Player, false);
					UpdateTiles(loc, data.entity as Player, true);
				}
				data.loc = loc;
			}
		}

		private void UpdateTiles(Vector2Int loc, Player player, bool onEnter = true)
		{
			Vector2Int forwardPos = loc + player.Facing().ToVector2Int();

			if (_tiles.ContainsKey(loc)) _tiles[loc].OnPlayerEnterRequest = onEnter;
			if (_tiles.ContainsKey(forwardPos) && !_tiles[forwardPos].HasWallAt(player.Facing().Opposite(), true)) _tiles[forwardPos].OnPlayerAdjancentRequest = onEnter;

			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					Vector2Int pos = new Vector2Int(loc.x + i, loc.y + j);
					if (_tiles.ContainsKey(pos))
					{
						if (pos != loc && pos != forwardPos) _tiles[pos].OnPlayerAdjancentRequest = false;
					}
				}
			}
			//Code for checking all tile Adjancent Check
			//    for (int i = -1; i <= 1; i++)
			//    {
			//        for (int j = -1; j <= 1; j++)
			//        {
			//            Vector2Int pos = new Vector2Int(loc.x + i, loc.y + j);
			//            if (_tiles.ContainsKey(pos))
			//            {
			//                if (i == 0 && j == 0)
			//                {
			//                    _tiles[pos].OnPlayerEnterRequest = onEnter;
			//                }
			//                else
			//                {
			//                    _tiles[pos].OnPlayerAdjancentRequest = onEnter;
			//                }
			//            }
			//        }
			//    }
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
						PlazmaDebug.LogWarning($"Trying to Instinate {tile.name} at ({tile.transform.position.x}, {tile.transform.position.z}).", "ERROR", 1);
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
						PlazmaDebug.LogWarning($"Trying to Add Interacteable {obj.name} at ({obj.transform.position.x}, {obj.transform.position.z}) But There Is No Tile.", "ERROR", 1);
					}
				}
			}
		}
		
		public Entity GetClosestEntity(Vector2Int pos, Func<Entity, bool> func)
		{
			HashSet<Vector2Int> visited = new() { { pos } };
			Queue<Vector2Int> nodes = new();
			nodes.Enqueue(pos);
			while (nodes.Count > 0)
			{
				Vector2Int v = nodes.Dequeue();
				List<EntityData> onTile = GetEntitesOnTile(v);
				if (onTile.Count > 0)
				{
					Entity entity = onTile.FirstOrDefault(e => func(e.entity))?.entity;
					if (entity) return entity;
				}
				DirectionExt.AllDirections().ForEach(dir =>
				{
					if (!CanMoveTo(v, dir)) return;
					Vector2Int w = v + dir.ToVector2Int();
					if (!visited.Add(w)) return;
					nodes.Enqueue(w);
				});
			}

			return null;
		}
		
		public List<Vector2Int> PathFind(Vector2Int from, Vector2Int to)
		{
			Dictionary<Vector2Int, Vector2Int> visited = new() { { from, from } };
			Queue<Vector2Int> nodes = new();
			nodes.Enqueue(from);
			while (nodes.Count > 0)
			{
				Vector2Int v = nodes.Dequeue();
				if (v == to)
				{
					List<Vector2Int> path = new() { v };
					while (v != from)
					{
						v = visited[v];
						path.Add(v);
					}

					path.Reverse();
					return path;
				}
				DirectionExt.AllDirections().ForEach(dir =>
				{
					if (!CanMoveTo(v, dir)) return;
					Vector2Int w = v + dir.ToVector2Int();
					if (!visited.TryAdd(w, v)) return;
					nodes.Enqueue(w);
				});
			}
			return new List<Vector2Int>();
		}

		private void Awake()
		{
			_entities = new List<EntityData>();
		}

		private void Start()
		{
			RegisterTiles();
			RegisterInteractables();
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Torch"))
            {
                Transform t = go.transform;
                Vector2Int pos = WorldToGrid(t.position);
                if (_tiles.TryGetValue(pos, out var tile)) t.parent = tile.transform;
                else
                {
                    foreach (Direction dir in DirectionExt.AllDirections())
                    {
                        if (_tiles.TryGetValue(pos + dir.ToVector2Int(), out tile))
                        {
                            t.parent = tile.transform;
                            break;
                        }
                        else if (_tiles.TryGetValue(pos + dir.ToVector2Int() + dir.Right().ToVector2Int(), out tile))
                        {
                            t.parent = tile.transform;
                            break;
                        }
                    }
                }
            }
		}

        private void FixedUpdate()
        {
            Vector2Int playerPos = DCGameManager.PlayerController.GridPosition();
            if (_lastPlayerPos == playerPos) return;
            _lastPlayerPos = playerPos;
            int maxDist = DCGameManager.settings.viewDistance;
            _tiles.ForEach(t => t.Value.distanceToPlayer = 10000000);
            CalcTilePlayerDist();
            foreach (var (pos, tile) in _tiles)
            {
                if (tile.distanceToPlayer <= DCGameManager.settings.viewDistance)
                {
                    tile.gameObject.SetActive(true);
                }
                else
                {
                    bool t = false;
                    foreach (Direction dir in DirectionExt.AllDirections())
                    {
                        Vector2Int np = pos + dir.ToVector2Int();
                        if (_tiles.ContainsKey(np) && _tiles[np].distanceToPlayer <= DCGameManager.settings.viewDistance)
                        {
                            tile.gameObject.SetActive(true);
                            t = true;
                        }
                    }
                    if (!t) tile.gameObject.SetActive(false);
                }
            }
        }

        private void CalcTilePlayerDist()
        {
            Vector2Int pos = DCGameManager.PlayerController.GridPosition();
			Dictionary<Vector2Int, int> visited = new() { { pos, 0 } };
			Queue<Vector2Int> nodes = new();
			nodes.Enqueue(pos);
			while (nodes.Count > 0)
			{
				Vector2Int v = nodes.Dequeue();
                int dist = visited[v];
				if (dist > DCGameManager.settings.viewDistance)
                {
                    break;
                }
				DirectionExt.AllDirections().ForEach(dir =>
				{
					if (!CanMoveTo(v, dir, false, true)) return;
					Vector2Int w = v + dir.ToVector2Int();
                    if (!visited.TryAdd(w, dist + 1))
                    {
                        if (visited[w] > dist + 1) visited[w] = dist + 1;
                        return;
                    }
					nodes.Enqueue(w);
				});
			}
            visited.ForEach(v => _tiles[v.Key].distanceToPlayer = v.Value);
        }
    }
}
