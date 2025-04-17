using PlazmaGames.Attribute;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public class TestItem : MonoBehaviour, IInteractable
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

        [SerializeField, ReadOnly] bool _isEntered;
        [SerializeField, ReadOnly] bool _isAdjancent;

        private List<Tile> _currentTiles;

        public void OnPlayerAdjancentEnter()
        {
            Debug.Log("On Adjancent Enter");
        }

        public void OnPlayerAdjancentExit()
        {
            Debug.Log("On Adjancent Exit");
        }

        public void OnPlayerEnter()
        {
            Debug.Log("On Enter");
        }

        public void OnPlayerExit()
        {
            Debug.Log("On Exit");
        }

        public void OnPressedDown() { }
        public void OnPressedUp() { }
        public void OnHover() { }

        private void FixedUpdate()
        {
            _isEntered = IsEntered;
            _isAdjancent = IsAdjancent;
        }

        private void LateUpdate()
        {
            WasStateEnterChangedThisFrame = false;
            WasStateAdjancentChangedThisFrame = false;
        }
    }
}
