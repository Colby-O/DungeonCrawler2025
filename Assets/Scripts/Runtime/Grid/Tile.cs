using System;
using System.Collections.Generic;
using UnityEngine;
using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.Attribute;
using System.Security.Cryptography;

namespace DC2025
{
	public class Tile : MonoBehaviour
	{
		[SerializeField] private SerializableDictionary<Direction, bool> _walls;
		[SerializeField, ReadOnly] private Vector3 _globalPosition;

        private List<IInteractable> _interactables;

        public bool OnPlayerEnterRequest { get; set; }
        public bool OnPlayerAdjancentRequest { get; set; }

        private bool _lastEnterState = false;
        private bool _lastAdjancentState = false;

        public bool HasItemWithCollider()
        {
            bool hasCollider = false;

            if (_interactables == null) return hasCollider;

            foreach (IInteractable interactable in _interactables)
            {
                hasCollider |= interactable.HasCollider;
            }

            return hasCollider;
        }

        public bool HasWallAt(Direction dir)
		{
            if (HasItemWithCollider()) return true;

			dir = dir.GetFacingDirection(-transform.rotation.eulerAngles.y);
			if (_walls.ContainsKey(dir)) return _walls[dir];
			return false;
		}

        public void AddInteractable(IInteractable interactable)
        {
            if (_interactables != null) _interactables.Add(interactable);
            else _interactables = new List<IInteractable>() { interactable };
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            if (_interactables != null && _interactables.Contains(interactable))
            {
                _interactables.Remove(interactable);
            }
        }

        public void OnPlayerEnter()
		{
            if (_interactables == null) return;

			foreach (IInteractable interactable in _interactables)
			{
                if (!interactable.IsEntered)
                {
                    interactable.OnPlayerEnter();
                    interactable.IsEntered = true;
                }
			}
		}

        public void OnPlayerExit()
        {
            if (_interactables == null) return;

            foreach (IInteractable interactable in _interactables)
            {
                if (interactable.IsEntered)
                {
                    interactable.OnPlayerExit();
                    interactable.IsEntered = false;
                }
            }
        }

        public void OnPlayerAdjancentEnter()
        {
            if (_interactables == null) return;

            foreach (IInteractable interactable in _interactables)
            {
                if (!interactable.IsAdjancent)
                {
                    interactable.OnPlayerAdjancentEnter();
                    interactable.IsAdjancent = true;
                }
            }
        }

        public void OnPlayerAdjancentExit()
        {
            if (_interactables == null) return;

            foreach (IInteractable interactable in _interactables)
            {
                if (interactable.IsAdjancent)
                {
                    interactable.OnPlayerAdjancentExit();
                    interactable.IsAdjancent = false;
                }
            }
        }

        private void FixedUpdate()
        {
            _globalPosition = transform.position;
        }

        private void Update()
        {
            if (_lastEnterState != OnPlayerEnterRequest)
            {
                if (OnPlayerEnterRequest) OnPlayerEnter();
                else OnPlayerExit();
            }

            if (_lastAdjancentState != OnPlayerAdjancentRequest)
            {
                if (OnPlayerAdjancentRequest) OnPlayerAdjancentEnter();
                else OnPlayerAdjancentExit();
            }

            _lastAdjancentState = OnPlayerAdjancentRequest;
            _lastEnterState = OnPlayerEnterRequest;
        }
    }
}
