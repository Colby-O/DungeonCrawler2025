using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public interface IInteractable
    {
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }
        public bool WasStateEnterChangedThisFrame { get; set; }
        public bool WasStateAdjancentChangedThisFrame { get; set; }
        public List<Tile> CurrentTile { get; }
        public bool HasCollider { get; }

        public void OnPlayerEnter();
        public void OnPlayerExit();
        public void OnPlayerAdjancentEnter();
        public void OnPlayerAdjancentExit();

        public void OnPressedDown();
        public void OnPressedUp();
        public void OnHover();
    }
}
