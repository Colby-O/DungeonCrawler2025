using DC2025.Utils;
using PlazmaGames.Core;
using UnityEngine;

namespace DC2025
{
    public class PickupableItem : MonoBehaviour, IInteractable
    {
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }

        public bool HasCollider => false;

        private IGridMonoSystem _grid;
        private Player _player;

        private Vector3 _centerPos;

        public void OnPlayerAdjancentEnter() { }

        public void OnPlayerAdjancentExit() { }

        public void OnPlayerEnter() 
        {
            _centerPos = _grid.GridToWorld(_grid.WorldToGrid(transform.position));
        }

        public void OnPlayerExit() 
        {
            transform.position = _centerPos.SetY(transform.position.y);
        }

        private void MoveItem()
        {
            if (IsEntered)
            {
                Vector3 center = _grid.GridToWorld(_grid.WorldToGrid(_centerPos));
                Vector2 tileSize = _grid.GetTileSize();

                transform.position = center + new Vector3(tileSize.x / 2.0f * _player.Facing().ToVector3().x, transform.position.y, tileSize.y / 2.0f * _player.Facing().ToVector3().z);
            }
        }

        private void Start()
        {
            _grid = GameManager.GetMonoSystem<IGridMonoSystem>();
            _player = DCGameManager.Player.GetComponent<Player>();
            _centerPos = _grid.GridToWorld(_grid.WorldToGrid(transform.position));
            transform.position = _centerPos.SetY(transform.position.y);
        }

        private void Update() 
        {
            MoveItem();
        }
    }
}
