using PlazmaGames.Attribute;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public class RestartTrigger : MonoBehaviour, IInteractable
    {
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }
        public bool WasStateEnterChangedThisFrame { get; set; }
        public bool WasStateAdjancentChangedThisFrame { get; set; }
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

        public bool HasCollider { get { return false; } }

        private List<Tile> _currentTiles;

        public void OnPlayerAdjancentEnter() { }

        public void OnPlayerAdjancentExit() { }

        public void OnPlayerExit() { }

        public void OnPressedDown() { }
        public void OnPressedUp() { }
        public void OnHover() { }

        public void OnPlayerEnter() 
        {
            Debug.Log("Restart");
            DCGameManager.OnRestart.Invoke();
        }

        private void LateUpdate()
        {
            WasStateEnterChangedThisFrame = false;
            WasStateAdjancentChangedThisFrame = false;
        }
    }
}
