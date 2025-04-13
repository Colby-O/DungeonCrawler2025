using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace DC2025
{
    public class TimingBar : MonoBehaviour
    {
        private RectTransform _panel;
        private RectTransform _ball;
        private RectTransform _targetBg;
        private RectTransform _targetLine1;
        private RectTransform _targetLine2;
        
        [SerializeField] private float _speed = 20;
        private float _dir = 1;

        private bool _stopped = false;

        private void Start()
        {
            _panel = transform.Find("Panel").GetComponent<RectTransform>();
            _ball = transform.Find("Ball").GetComponent<RectTransform>();
            _targetBg = transform.Find("TargetBg").GetComponent<RectTransform>();
            _targetLine1 = transform.Find("TargetLine1").GetComponent<RectTransform>();
            _targetLine2 = transform.Find("TargetLine2").GetComponent<RectTransform>();
        }
        
        private void Update()
        {
            if (!_stopped)
            {
                _ball.Translate(new Vector3(Time.deltaTime * _speed * _dir, 0, 0));
                if (_ball.anchoredPosition.x + _ball.rect.size.x >= _panel.anchoredPosition.x + _panel.rect.size.x) _dir = -1;
                else if (_ball.anchoredPosition.x <= _panel.anchoredPosition.x) _dir = 1;
            }
        }

        public bool IsGreen()
        {
            return (
                _ball.anchoredPosition.x + _ball.rect.size.x >= _targetBg.anchoredPosition.x &&
                _ball.anchoredPosition.x <= _targetBg.anchoredPosition.x + _targetBg.rect.size.x
            );
        }

        public bool Stop()
        {
            _stopped = true;
            return IsGreen();
        }

        public void Reset()
        {
            _ball.anchoredPosition = new Vector2(_targetBg.anchoredPosition.x, _ball.anchoredPosition.y);
            _stopped = false;
        }

        public bool IsStopped() => _stopped;
    }
}
