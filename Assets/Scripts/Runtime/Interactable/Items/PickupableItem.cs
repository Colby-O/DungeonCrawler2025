using System;
using DC2025.Utils;
using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

namespace DC2025
{
    public abstract class PickupableItem : MonoBehaviour, IInteractable
    {
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }
        public bool WasStateEnterChangedThisFrame { get; set; }
        public bool WasStateAdjancentChangedThisFrame { get; set; }

        protected virtual bool DoNotRotate() => false;
        
        public List<Tile> CurrentTile
        {
            get
            {
                if (_currentTiles == null)
                {
                    _currentTiles = new List<Tile>();
                }

                return _currentTiles;
            }
        }

        public bool HasCollider => false;

        private IGridMonoSystem _grid;
        private IInventoryMonoSystem _inventory;

        [Header("DeBugging")]
        [SerializeField, ReadOnly] private Player _player;
        [SerializeField, ReadOnly] private Vector3 _centerPos;
        [SerializeField, ReadOnly] private float _height;
        [SerializeField, ReadOnly] private bool _hasInited = false;

        private List<Tile> _currentTiles;

        public UnityEvent OnPickup = new UnityEvent();
        public UnityEvent OnDrop = new UnityEvent();

        protected virtual float RotationOffset()
        {
            return 0f;
        }

        private void MoveItem()
        {
            if (_player == null) _player = DCGameManager.PlayerController;

            if (IsEntered && !DoNotRotate())
            {
                Vector3 center = _grid.GridToWorld(_grid.WorldToGrid(_centerPos));
                Vector2 tileSize = _grid.GetTileSize();

                transform.position = center + new Vector3(tileSize.x / 2.0f * _player.Facing().ToVector3().x, transform.position.y, tileSize.y / 2.0f * _player.Facing().ToVector3().z);
                transform.position = transform.position.SetY(_height);
            }
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetY(DCGameManager.PlayerController.Facing().GetFacing() + RotationOffset()));
        }

        protected virtual bool CanPickup() => true;

        private void PickupItem()
        {
            if (_inventory.GetMouseSlot().HasItem()) return;
            if (!CanPickup()) return;

            OnPickup.Invoke();
            _inventory.GetMouseSlot().UpdateSlot(this);
            Hide();
        }

        private void Recenter()
        {
            transform.position = _centerPos.SetY(_height);
        }

        public bool Drop()
        {
            Vector2Int playerPos = _grid.WorldToGrid(DCGameManager.Player.transform.position);
            Tile playerTile = _grid.GetTileAt(playerPos);
            bool isTileFree = playerTile != null && !playerTile.HasInteractableOfType<PickupableItem>();
            if (isTileFree)
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.dropSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                gameObject.SetActive(true);
                playerTile.AddInteractable(this);
                _centerPos = _grid.GridToWorld(playerPos);
                IsEntered = true;
                MoveItem();
                OnDrop.Invoke();
            }
            else
            {
                GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send($"You try and place a {GetName()} but find that there is not enough room.");
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.uiClickSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
            }

            return isTileFree;
        }

        public void Hide()
        {
            foreach (Tile tile in CurrentTile.ToList()) tile?.RemoveInteractable(this);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Hide();
        }

        public void ForceInit()
        {
            if (_hasInited) return;
            _hasInited = true;
            _height = transform.position.y;
            _grid = GameManager.GetMonoSystem<IGridMonoSystem>();
            _inventory = GameManager.GetMonoSystem<IInventoryMonoSystem>();
            if (DCGameManager.Player != null) _player = DCGameManager.PlayerController;
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

        public static PickupableItem InstantiateItemOfType(ItemDataType type, Dictionary<string, object> data)
        {
            PickupableItem item = null;

            if (type == ItemDataType.RawCrafting)
            {
                item = Instantiate(Resources.Load<RawCraftingItem>("Prefabs/Items/RawCraftingMaterial"));
                (item as RawCraftingItem).SetMaterial((MaterialType)data["Material"]);
            }
            else if (type == ItemDataType.Bucket)
            {
                item = Instantiate(Resources.Load<BucketItem>("Prefabs/Items/BucketItem"));
                (item as BucketItem).SetMaterial((MaterialType)data["Material"]);
                (item as BucketItem).SetRating((int)data["Rating"]);
            }
            else if (type == ItemDataType.Mold)
            {
                item = Instantiate(Resources.Load<MoldItem>($"Prefabs/Items/Molds/{(BladeType)data["Blade"]}Mold"));
                (item as MoldItem).bladeType = (BladeType)data["Blade"];
            }
            else if (type == ItemDataType.UnfBlade)
            {
                item = GameManager.GetMonoSystem<ISwordBuilderMonoSystem>().CreateUnfBlade((BladeType)data["Blade"], (MaterialType)data["Material"], (int)data["Rating"]);
            }
            else if (type == ItemDataType.Blade)
            {
                item = GameManager.GetMonoSystem<ISwordBuilderMonoSystem>().CreateBlade((BladeType)data["Blade"], (MaterialType)data["Material"], (int)data["Rating"]);
            }
            else if (type == ItemDataType.Weapon)
            {
                item = GameManager.GetMonoSystem<ISwordBuilderMonoSystem>().CreateSword((BladeType)data["Blade"], (HandleType)data["Handle"],(MaterialType)data["Material"], (int)data["Rating"]);
                (item as WeaponItem).SetDurability((float)data["Durability"]);
            }
            else if (type == ItemDataType.Potion)
            {
                item = Instantiate(Resources.Load<PotionItem>("Prefabs/Items/PotionItem"));
                (item as PotionItem).SetMaterial((MaterialType)data["Material"]);
            }
            else if (type == ItemDataType.Key)
            {
                item = Instantiate(Resources.Load<KeyItem>("Prefabs/Items/Key"));
                (item as KeyItem).SetMaterial((MaterialType)data["Material"]);
            }

            if (item != null) item.gameObject.SetActive(false);

            return item;
        }

        public virtual Color GetColor() => Color.white;

        public virtual void OnPressedUp() { }

        public virtual void OnHover() { }

        public virtual void OnPlayerAdjancentEnter() { }

        public virtual void OnPlayerAdjancentExit() { }

        public virtual void OnPressedDown()
        {
            if (GameManager.GetMonoSystem<IFightMonoSystem>().InFight() || GameManager.GetMonoSystem<IInventoryMonoSystem>().GetMouseSlot().HasItem()) return;
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.pickupSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
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

        protected virtual void OnEnable()
        {
            if (!_hasInited) ForceInit();
        }

        protected virtual void Start()
        {
            ForceInit();
        }

        private void Update() 
        {
            MoveItem();
        }

        private void LateUpdate()
        {
            WasStateEnterChangedThisFrame = false;
            WasStateAdjancentChangedThisFrame = false;
        }
    }
}
