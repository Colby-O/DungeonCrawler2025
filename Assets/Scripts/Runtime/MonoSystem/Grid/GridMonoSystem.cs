using UnityEngine;

namespace DC2025
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [SerializeField] private Vector2 _tileSize;

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
    }
}
