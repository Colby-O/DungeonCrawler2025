using DC2025.Utils;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using UnityEngine;

namespace DC2025
{
    public abstract class PickupableItem : MonoBehaviour, IInteractable
    {
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }
        public Tile CurrentTile { get; set; }

        public bool HasCollider => false;

        private IGridMonoSystem _grid;
        private IInventoryMonoSystem _inventory;

        [Header("DeBugging")]
        [SerializeField, ReadOnly] private Player _player;
        [SerializeField, ReadOnly] private Vector3 _centerPos;
        [SerializeField, ReadOnly] private float _height;
        [SerializeField, ReadOnly] private bool _hasInited = false;

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
            Hide();
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

        public void Hide()
        {
            CurrentTile?.RemoveInteractable(this);
            gameObject.SetActive(false);
        }

        public void ForceInit()
        {
            if (_hasInited) return;
            _hasInited = true;
            _height = transform.position.y;
            _grid = GameManager.GetMonoSystem<IGridMonoSystem>();
            _inventory = GameManager.GetMonoSystem<IInventoryMonoSystem>();
            _player = DCGameManager.Player.GetComponent<Player>();
            _centerPos = _grid.GridToWorld(_grid.WorldToGrid(transform.position));
            transform.position = _centerPos.SetY(transform.position.y);
        }

        public void PlaceItemAt(Vector2Int pos)
        {

        }

        public void Release()
        {
            Hide();
            Destroy(gameObject);
        }

        public abstract Sprite GetIcon();

        public abstract string GetDescription();
        public abstract string GetName();

        public virtual Color GetColor() => Color.white;

        public virtual void OnPressedUp() { }

        public virtual void OnHover() { }

        public virtual void OnPlayerAdjancentEnter() { }

        public virtual void OnPlayerAdjancentExit() { }

        public virtual void OnPressedDown()
        {
            PickupItem();
        }

        public virtual void OnPlayerEnter()
        {
            _centerPos = _grid.GridToWorld(_grid.WorldToGrid(transform.position));
        }

        public virtual void OnPlayerExit()
        {
            Recenter();
        }

        private void Start()
        {
            ForceInit();
        }

        private void Update() 
        {
            MoveItem();
        }
    }
}
