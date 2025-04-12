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
    }
}
