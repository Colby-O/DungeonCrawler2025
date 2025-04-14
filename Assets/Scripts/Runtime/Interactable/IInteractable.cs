using UnityEngine;

namespace DC2025
{
    public interface IInteractable
    {
        public Tile CurrentTile { get; set; }
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }

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
