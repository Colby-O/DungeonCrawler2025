using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public abstract class Blockage : MonoBehaviour, IInteractable
    {
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
            Interact();
        }

        public virtual void Interact()
        {
            if (!IsOpen) Open();
            else Close();
        }

        public virtual bool CanOpen()
        {
            return !IsLocked;
        }

        public abstract void Open();
        public abstract void Close();
        public abstract void Lock();
        public abstract void Unlock();

        private void LateUpdate()
        {
            WasStateEnterChangedThisFrame = false;
            WasStateAdjancentChangedThisFrame = false;
        }
    }
}
