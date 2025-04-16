using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlazmaGames.Animation;
using PlazmaGames.Core;
using Unity.VisualScripting;

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

        public bool HasSword() => _model;
        
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
        
        public void Stumble()
        {
            if (!HasSword()) return;
            Quaternion startRot = _model.rotation;
            Quaternion endRot = Quaternion.Euler(0, 20, 20) * startRot;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                this.stats.speed,
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

        public void SetModel(Transform model)
        {
            if (!model)
            {
                if (HasSword()) Destroy(_model.gameObject);
                _model = null;
            }
            else
            {
                if (HasSword()) Destroy(_model.gameObject);
                _model = Instantiate(model.gameObject, _swordObject).transform;
                Lower();
            }
        }
    }
}
