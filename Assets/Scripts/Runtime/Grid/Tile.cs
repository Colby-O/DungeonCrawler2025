using System;
using System.Collections.Generic;
using UnityEngine;
using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.Attribute;
using System.Security.Cryptography;
using System.Linq;

namespace DC2025
{
	public class Tile : MonoBehaviour
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        [SerializeField] private SerializableDictionary<Direction, bool> _walls;
        [SerializeField] private SerializableDictionary<Direction, Blockage> _blockages;
        [SerializeField, ReadOnly] private Vector3 _globalPosition;
        [SerializeField, ReadOnly] private Direction _doorFacing;

        private MeshRenderer _highlight;

        private float _enemySeenTimer;

        private List<IInteractable> _interactables;

        public bool OnPlayerEnterRequest { get; set; }
        public bool OnPlayerAdjancentRequest { get; set; }

        private bool _lastEnterState = false;
        private bool _lastAdjancentState = false;

        public bool HasInteractable() => _interactables != null && _interactables.Count > 0;
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

        public bool HasWallAt(Direction dir, bool ignoreColliders = false)
		{
            if (HasItemWithCollider() && !ignoreColliders) return true;

			dir = dir.GetFacingDirection(-transform.rotation.eulerAngles.y);
			if ((_walls.ContainsKey(dir) && _walls[dir]) || (_blockages.ContainsKey(dir) && _blockages[dir] != null && !_blockages[dir].IsOpen)) return true;
			return false;
		}

        public void AddInteractable(IInteractable interactable)
        {
            if (_interactables == null) _interactables = new List<IInteractable>();

            if (!_interactables.Contains(interactable))
            {
                _interactables.Add(interactable);
                interactable.CurrentTile.Add(this);
            }
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            if (_interactables != null && _interactables.Contains(interactable))
            {
                _interactables.Remove(interactable);
                interactable.CurrentTile.Remove(this);
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

                interactable.WasStateEnterChangedThisFrame = true;
            }
		}

        public void OnPlayerExit()
        {
            if (_interactables == null) return;

            foreach (IInteractable interactable in _interactables)
            {
                if (interactable.IsEntered && !interactable.WasStateEnterChangedThisFrame)
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
                interactable.WasStateAdjancentChangedThisFrame = true;
            }
        }

        public void OnPlayerAdjancentExit()
        {
            if (_interactables == null) return;

            foreach (IInteractable interactable in _interactables)
            {
                if (interactable.IsAdjancent && !interactable.WasStateAdjancentChangedThisFrame)
                {
                    interactable.OnPlayerAdjancentExit();
                    interactable.IsAdjancent = false;
                }
            }
        }

        public SerializableDictionary<Direction, Blockage> GetBlockages()
        {
            return _blockages;
        }

        private void Start()
        {
            _highlight = transform.Find("Highlight").GetComponent<MeshRenderer>();
            _highlight.material.EnableKeyword("_EMISSION");

            if (_blockages != null)
            {
                foreach (Blockage blockage in _blockages.Values)
                {
                    if (blockage == null) continue;
                    AddInteractable(blockage);
                }
            }
        }

        private void FixedUpdate()
        {
            _globalPosition = transform.position;
            _enemySeenTimer -= Time.fixedDeltaTime * 5f;
            _highlight.material.SetColor(EmissionColor, Color.Lerp(Color.black, DCGameManager.settings.enemyVisionHighlightColor, _enemySeenTimer));
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

        public void SetEnemySeen()
        {
            _enemySeenTimer = 1;
        }

    }
}
