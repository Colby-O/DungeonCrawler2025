using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlazmaGames.Animation;
using PlazmaGames.Core;

namespace DC2025
{
    public class SwordSwing : MonoBehaviour
    {
        private Transform _model;
        private Transform _defaultPosition;
        private Transform _blockPosition;
        [SerializeField] private List<Transform> _swingPositions;
        [SerializeField] private float _swingTime = 0.64f;
        
        public void Raise()
        {
            _model.SetPositionAndRotation(_swingPositions[0].position, _swingPositions[0].rotation);
        }
        
        public void Block()
        {
            _model.SetPositionAndRotation(_blockPosition.position, _blockPosition.rotation);
        }

        public void Lower()
        {
            _model.SetPositionAndRotation(_defaultPosition.position, _defaultPosition.rotation);
        }

        public void Swing()
        {
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _swingTime,
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
            Quaternion startRot = _model.rotation;
            Quaternion endRot = Quaternion.Euler(0, 20, 20) * startRot;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _swingTime,
                (t) =>
                {
                    if (t < 0.5) _model.rotation = Quaternion.Lerp(startRot, endRot, t * 2);
                    else _model.rotation = Quaternion.Lerp(endRot, startRot, (t - 0.5f) * 2);
                }
            );
        }

        private void Start()
        {
            _model = transform.Find("Model");
            Transform positions = transform.Find("Positions");
            _defaultPosition = positions.Find("Default");
            _blockPosition = positions.Find("Block");
            positions = positions.Find("Swing");
            _swingPositions = new List<Transform>();
            for (int i = 0; i < positions.childCount; i++)
            {
                _swingPositions.Add(positions.GetChild(i));
            }
        }

    }
}
