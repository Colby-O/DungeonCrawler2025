using DC2025.Utils;
using PlazmaGames.Animation;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace DC2025
{
    public class Door : Blockage
    {
        [Header("References")]
        [SerializeField] private Transform _hinge;
        [SerializeField] private Transform _center;

        [Header("Settings")]
        [SerializeField] private float _openSpeed;
        [SerializeField] private bool _isLocked;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private bool _isOpen;
        [SerializeField, ReadOnly] private bool _inProgress;

        public void Open(Transform from)
        {
            if (IsOpen || _inProgress || IsLocked) return;

            IsOpen = true;
            _inProgress = true;

            float start = Math.NormalizeAngle(_hinge.localRotation.eulerAngles.y);
            float target = 90f;

            if (Vector3.Dot(_center.right, (_center.position - from.position).normalized) > 0)
            {
                target -= 180f;
            }

            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _openSpeed,
                (float t) =>
                {
                    _hinge.localRotation = Quaternion.Euler(0, Mathf.Lerp(start, target, t), 0);
                },
                () =>
                {
                    _inProgress = false;
                }
            );
        }

        public override void Open()
        {
            Open(DCGameManager.Player.transform);
        }

        public override void Close()
        {
            if (!IsOpen || _inProgress || IsLocked) return;

            IsOpen = false;
            _inProgress = true;
            float start = Math.NormalizeAngle(_hinge.localRotation.eulerAngles.y);
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _openSpeed,
                (float t) =>
                {
                    _hinge.localRotation = Quaternion.Euler(0, start - t * start, 0);
                },
                () =>
                {
                    _inProgress = false;
                }
            );
        }

        public override void Lock()
        {
            IsLocked = true;
        }

        public override void Unlock()
        {
            IsLocked = false;
        }

        private void Awake()
        {
            if (_isLocked) Lock();
        }
    }
}
