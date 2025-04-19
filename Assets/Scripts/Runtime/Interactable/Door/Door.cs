using DC2025.Utils;
using PlazmaGames.Animation;
using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public class Door : Blockage
    {
        [Header("References")]
        [SerializeField] private Transform _hinge;
        [SerializeField] private Transform _center;

        [Header("Settings")]
        [SerializeField] private float _openSpeed;
        [SerializeField] private List<KeyPad> _keysNeeded;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private bool _isOpen;
        [SerializeField, ReadOnly] private bool _inProgress;

        public void Open(Transform from)
        {
            Unlock();
            if (IsOpen || _inProgress || IsLocked) return;

            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.openDoorSound, PlazmaGames.Audio.AudioType.Sfx, false, true);

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

            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.closeDoorSound, PlazmaGames.Audio.AudioType.Sfx, false, true);

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
            if (!IsLocked) return;
            bool canUnlock = true;

            foreach (KeyPad pad in _keysNeeded)
            {
                if (pad.IsLocked() && GameManager.GetMonoSystem<IInventoryMonoSystem>().HasKeyOfType(pad.GetMaterial())) pad.Unlock();
            }

            foreach (KeyPad pad in _keysNeeded) canUnlock &= !pad.IsLocked();

            IsLocked = !canUnlock;

            if (IsLocked)
            {
                string keysNeeded = string.Empty;
                foreach (KeyPad pad in _keysNeeded) if (pad.IsLocked()) keysNeeded += $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[pad.GetMaterial()])}>{pad.GetMaterial()}</color> ";

                GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send($"You try to open the door but fail. You need key(s) of type {keysNeeded}to unlock the door.");
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.lockedSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
            }
            else
            {
                GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send("You successfully unlocked the door and can now proceed.");
                foreach (KeyPad pad in _keysNeeded) pad.gameObject.SetActive(false);
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.unlockedSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
            }
        }

        public override bool CanOpen()
        {
            return true;
        }

        private void Start()
        {
            IsLocked = _keysNeeded != null && _keysNeeded.Count > 0;
        }
    }
}
