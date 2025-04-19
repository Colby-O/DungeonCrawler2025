using PlazmaGames.Animation;
using PlazmaGames.Core;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

namespace DC2025
{
    public abstract class Station : MonoBehaviour, IInteractable
    {
        [Header("View Data")]
        [SerializeField] private Transform _offset;
        [SerializeField] private float _transitionSpeed = 0.2f;

        private List<Tile> _currentTiles;

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
        public bool WasStateEnterChangedThisFrame { get; set; }
        public bool WasStateAdjancentChangedThisFrame { get; set; }
        public bool IsEnabled { get; protected set; }

        public bool HasCollider { get { return true; } }
        
        private void TransitionStep(float t, Camera cam, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
        {
            cam.transform.position = Vector3.Lerp(startPos, endPos, t);
            cam.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
        }

        protected void StartTransition()
        {
            Camera cam = DCGameManager.Player.GetCamera();
            Vector3 startPos = cam.transform.position;
            Quaternion startRot = cam.transform.rotation;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _transitionSpeed,
                (float t) => TransitionStep(
                    t, cam, startPos, 
                    (IsEnabled) ? _offset.position : DCGameManager.Player.GetCameraLoc().Item1, startRot, 
                    (IsEnabled) ? _offset.rotation : DCGameManager.Player.GetCameraLoc().Item2
                )
            );
        }

        public abstract void Interact();

        public abstract void ForceClose();

        public void OnPressedUp() { }

        public virtual void OnHover() { }

        public virtual void OnPlayerEnter() { }

        public virtual void OnPlayerExit() { }

        public virtual void OnPressedDown()
        {
            if (!GameManager.GetMonoSystem<IInventoryMonoSystem>().GetMouseSlot().HasItem())
            {
                Interact();
            }
        }

        protected virtual void OnClose()
        {
            GameManager.GetMonoSystem<IInventoryMonoSystem>().GetPopup().Disable();
        }

        public virtual void OnPlayerAdjancentEnter() 
        {
            DCGameManager.Player.NearbyStation = this;
        }

        public virtual void OnPlayerAdjancentExit() 
        {
            if (DCGameManager.Player.NearbyStation == this)
            {
                DCGameManager.Player.NearbyStation.ForceClose();
                DCGameManager.Player.NearbyStation = null;
            }
        }

        private void LateUpdate()
        {
            WasStateEnterChangedThisFrame = false;
            WasStateAdjancentChangedThisFrame = false;
        }
    }
}
