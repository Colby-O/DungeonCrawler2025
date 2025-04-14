using DC2025.Utils;
using PlazmaGames.Core;
using UnityEngine;

namespace DC2025
{
    public class PickupableItem : MonoBehaviour, IInteractable
    {
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }
        public Tile CurrentTile { get; set; }

        public bool HasCollider => false;

        [Header("Data")]
        [SerializeField] private Item _data;

        private IGridMonoSystem _grid;
        private IInventoryMonoSystem _inventory;
        private Player _player;
        private Vector3 _centerPos;
        private float _height;

        public Item GetItemData() => _data;

        private void MoveItem()
        {
            if (IsEntered)
            {
                Vector3 center = _grid.GridToWorld(_grid.WorldToGrid(_centerPos));
                Vector2 tileSize = _grid.GetTileSize();

                transform.position = center + new Vector3(tileSize.x / 2.0f * _player.Facing().ToVector3().x, transform.position.y, tileSize.y / 2.0f * _player.Facing().ToVector3().z);
                transform.position = transform.position.SetY(_height);
            }
        }

        private void PickupItem()
        {
            if (_inventory.GetMouseSlot().HasItem()) return;

            _inventory.GetMouseSlot().UpdateSlot(this);
            HideItem();
        }

        private void Recenter()
        {
            transform.position = _centerPos.SetY(_height);
        }

        public void Drop()
        {
            gameObject.SetActive(true);
            (Vector2Int, Tile) newGridPos = _grid.FindVaildLocationNearPlayer();
            newGridPos.Item2.AddInteractable(this);
            _centerPos = _grid.GridToWorld(newGridPos.Item1);
            Vector2Int playerPos = _grid.WorldToGrid(DCGameManager.Player.transform.position);

            if (playerPos == newGridPos.Item1)
            {
                IsEntered = true;
            }
            else
            {
                IsEntered = false;
                Recenter();
            }
        }

        public void HideItem()
        {
            CurrentTile?.RemoveInteractable(this);
            gameObject.SetActive(false);
        }

        public void PlaceItemAt(Vector2Int pos)
        {

        }

        public void Release()
        {
            HideItem();
            Destroy(gameObject);
        }

        public void OnPressedUp() { }

        public void OnHover() { }

        public void OnPlayerAdjancentEnter() { }

        public void OnPlayerAdjancentExit() { }

        public void OnPressedDown()
        {
            PickupItem();
        }

        public void OnPlayerEnter()
        {
            _centerPos = _grid.GridToWorld(_grid.WorldToGrid(transform.position));
        }

        public void OnPlayerExit()
        {
            Recenter();
        }

        private void Awake()
        {
            _height = transform.position.y;
        }

        private void Start()
        {
            _grid = GameManager.GetMonoSystem<IGridMonoSystem>();
            _inventory = GameManager.GetMonoSystem<IInventoryMonoSystem>();
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
