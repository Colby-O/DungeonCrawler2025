using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlazmaGames.Animation;
using PlazmaGames.Core;
using Unity.VisualScripting;
using PlazmaGames.Audio;

namespace DC2025
{
    public class Sword : MonoBehaviour
    {
        private Transform _swordObject;
        private Transform _model = null;
        private Transform _defaultPosition;
        private Transform _blockPosition;
        [SerializeField] private List<Transform> _swingPositions;
        public SwordStats stats = new SwordStats();
        private WeaponItem _item = null;

        public bool HasSword() => _model;

        public BladeController GetBladeController() => _model.GetChild(0).GetComponent<BladeController>();

        public float Durability() => _item ? _item.GetDurability() : 0;
        public void TakeDurability()
        {
            if (_item)
            {
                _item.TakeDurability();
                if (Durability() <= 0)
                {
                    GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send("Your sword shatters into 1000 pieces.");
                    _item.Release();
                    GameManager.GetMonoSystem<IInventoryMonoSystem>().GetHandSlot(SlotType.Left).Clear();
                    SetModel(null);
                }
            }
        }

        public void Raise()
        {
            if (!HasSword()) return;
            _model.SetPositionAndRotation(_swingPositions[0].position, _swingPositions[0].rotation);
        }
        
        public void Block()
        {
            if (!HasSword()) return;
            _model.SetPositionAndRotation(_blockPosition.position, _blockPosition.rotation);
        }

        public void Lower()
        {
            if (!HasSword()) return;
            _model.SetPositionAndRotation(_defaultPosition.position, _defaultPosition.rotation);
        }

        public void Swing()
        {
            if (!HasSword()) return;

            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.swordSound, PlazmaGames.Audio.AudioType.Sfx, false, true);

            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                this.stats.speed,
                (t) =>
                {
                    t *= 1.11111f;
                    if (t < 1)
                    {
                        int count = _swingPositions.Count - 1;
                        int swingIndex = Mathf.FloorToInt(t * count);
                        swingIndex = Mathf.Clamp(swingIndex, 0, count - 1);
                        Transform from = _swingPositions[swingIndex];
                        Transform to = _swingPositions[swingIndex + 1];
                        float st = (t - (float)swingIndex / count) * count;
                        _model.position = Vector3.Lerp(from.position, to.position, st);
                        _model.rotation = Quaternion.Lerp(from.rotation, to.rotation, st);
                    }
                },
                () =>
                {
                    _model.SetPositionAndRotation(_defaultPosition.position, _defaultPosition.rotation);
                }
                );
        }
        
        public void Stumble(float aniSpeed)
        {
            if (!HasSword()) return;
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.entityStepSounds[Random.Range(0, DCGameManager.settings.entityStepSounds.Count)], PlazmaGames.Audio.AudioType.Sfx, false, true);
            Quaternion startRot = _model.rotation;
            Quaternion endRot = Quaternion.Euler(0, 20, 20) * startRot;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                aniSpeed,
                (t) =>
                {
                    if (t < 0.5) _model.rotation = Quaternion.Lerp(startRot, endRot, t * 2);
                    else _model.rotation = Quaternion.Lerp(endRot, startRot, (t - 0.5f) * 2);
                }
            );
        }

        private void Start()
        {
            _swordObject = transform.GetChild(0);
            _model = _swordObject.Find("Model");
            Transform positions = _swordObject.Find("Positions");
            _defaultPosition = positions.Find("Default");
            _blockPosition = positions.Find("Block");
            positions = positions.Find("Swing");
            _swingPositions = new List<Transform>();
            for (int i = 0; i < positions.childCount; i++)
            {
                _swingPositions.Add(positions.GetChild(i));
            }
        }

        public void SetModel(WeaponItem weapon)
        {
            if (!weapon)
            {
                if (HasSword()) Destroy(_model.gameObject);
                _model = null;
                _item = null;
                return;
            }
            
            Transform model = weapon.transform.Find("Model");
            _item = weapon;
            if (HasSword()) Destroy(_model.gameObject);
            _model = Instantiate(model.gameObject, _swordObject).transform;
            Lower();
            CalculateStats();
        }

        private void CalculateStats()
        {
            this.stats.damage = DCGameManager.settings.materialDamage[_item.GetMaterial()];
            this.stats.damage *= DCGameManager.settings.bladeDamageScales[_item.bladeType];
            this.stats.damage *= DCGameManager.settings.ratingDamageScales[_item.GetRating()];
            this.stats.damage *= DCGameManager.settings.handleDamageScales[_item.handleType];
            this.stats.speed = DCGameManager.settings.bladeSpeeds[_item.bladeType];
            this.stats.speed *= DCGameManager.settings.handleSpeedScales[_item.handleType];
            this.stats.staminaMultiplier = DCGameManager.settings.bladeStaminaScales[_item.bladeType];
            this.stats.staminaMultiplier *= DCGameManager.settings.handleStaminaScales[_item.handleType];
        }
    }
}
