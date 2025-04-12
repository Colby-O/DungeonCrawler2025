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
        public bool CanMoveTo(Vector2Int gridPos, Direction from);
        public void Sync(Entity entity, Vector2Int loc);
        public List<EntityData> GetEntitesOnTile(Vector2Int pos);
        public void SetTileEnemySeen(Vector2Int pos);
        public void RemoveEntity(Entity entity);
    }
}
