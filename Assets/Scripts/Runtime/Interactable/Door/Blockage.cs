using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.DataPersistence;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public abstract class Blockage : MonoBehaviour, IInteractable, IDataPersistence
    {
        private static int _instanceCount;

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

        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }
        public bool HasCollider => false;
        public bool WasStateEnterChangedThisFrame { get; set; }
        public bool WasStateAdjancentChangedThisFrame { get; set; }

        public bool IsOpen { get; set; }
        public bool IsLocked { get; set; }

        private List<Tile> _currentTiles = new List<Tile>();

        [SerializeField, ReadOnly] private int _id;
        [SerializeField, ReadOnly] private bool _lockedState;

        public virtual void OnHover() { }
        public virtual void OnPlayerAdjancentEnter() { }
        public virtual void OnPlayerAdjancentExit() { }
        public virtual void OnPressedUp() { }

        public virtual void OnPlayerEnter()
        {
            DCGameManager.Player.NearbyBlockage = this;
        }

        public virtual void OnPlayerExit()
        {
            if (DCGameManager.Player.NearbyBlockage == this) DCGameManager.Player.NearbyBlockage = null;
        }

        public virtual void OnPressedDown()
        {
            if (!GameManager.GetMonoSystem<IInventoryMonoSystem>().GetMouseSlot().HasItem())
            {
                Interact();
            }
        }

        public virtual void Interact()
        {
            if (!IsOpen) Open();
            else Close();
        }

        public virtual bool CanOpen()
        {
            return true;
        }

        public abstract void Open();
        public abstract void Close();
        public abstract void Lock();
        public abstract void Unlock();

        protected virtual void Awake()
        {
            _id = _instanceCount++;
            _lockedState = IsLocked;
        }

        private void LateUpdate()
        {
            WasStateEnterChangedThisFrame = false;
            WasStateAdjancentChangedThisFrame = false;
        }

        public bool SaveData<TData>(ref TData rawData) where TData : GameData
        {
            DCGameData data = rawData as DCGameData;
            if (data == null) return false;

            if (data.doorLockedStates.ContainsKey(UID.GetID(transform))) data.doorLockedStates[UID.GetID(transform)] = IsLocked;
            else data.doorLockedStates.Add(UID.GetID(transform), IsLocked);
            return true;
        }

        public bool LoadData<TData>(TData rawData) where TData : GameData
        {
            DCGameData data = rawData as DCGameData;
            if (data == null) return false;

            if (data.doorLockedStates.ContainsKey(UID.GetID(transform)))
            {
                IsLocked = data.doorLockedStates[UID.GetID(transform)];
            }
            else
            {
                IsLocked = _lockedState;
            }

            if (!IsLocked) Unlock();
            else Lock();

            return false;
        }
    }
}
