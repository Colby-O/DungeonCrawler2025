using System;
using System.Collections.Generic;
using PlazmaGames.Core.MonoSystem;
using UnityEngine;

namespace DC2025
{
    public interface IGridMonoSystem : IMonoSystem
    {
        public Vector2 GetTileSize();
        public Vector2Int WorldToGrid(Vector3 worldPos);
        public Vector3 GridToWorld(Vector2Int gridPos);
        public Tile GetTileAt(Vector2Int gridPos);
        public Tile GetTileAt(int x, int y);
        public bool CanMoveTo(Vector2Int gridPos, Direction dir, bool forceDoorsOpen = false, bool ignoreDoors = false);
        public void Sync(Entity entity, Vector2Int loc);
        public List<EntityData> GetEntitesOnTile(Vector2Int pos);
        public void SetTileEnemySeen(Vector2Int pos);
        public void SetTileDistraction(Vector2Int pos);
        public void UnsetTileDistraction(Vector2Int pos);
        public void RemoveEntity(Entity entity);
        public (Vector2Int, Tile) FindVaildLocationNearPlayer();
        public Entity GetClosestEntity(Vector2Int pos, Func<Entity, bool> func);
        public List<Vector2Int> PathFind(Vector2Int from, Vector2Int to);
    }
}
