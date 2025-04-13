using PlazmaGames.Attribute;
using PlazmaGames.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DC2025
{
    public class GenericView : View
    {
        [Header("Chat Window")]
        [SerializeField] private float _scrollRate = 0.05f;
        [SerializeField] private ScrollRect _container;
        [SerializeField] private EventButton _scrollUp;
        [SerializeField] private EventButton _scrollDown;

        [SerializeField, ReadOnly] private int _scrollDir = 0;

        IEnumerator ChangeScrollRect(float val)
        {
            yield return new WaitForEndOfFrame();
            _container.verticalNormalizedPosition = val;
        }

        public void ScrollToBottom()
        {
            StartCoroutine(ChangeScrollRect(0));
        }

        public void ScrollToTop()
        {
            StartCoroutine(ChangeScrollRect(1));
        }

        public override void Init()
        {
            _scrollDown.onPointerDown.AddListener(() => _scrollDir = -1);
            _scrollDown.onPointerUp.AddListener(() => _scrollDir = 0);
            _scrollUp.onPointerDown.AddListener(() => _scrollDir = 1);
            _scrollUp.onPointerUp.AddListener(() => _scrollDir = 0);

        }

        public override void Hide() { }

        private void Scroll(int dir)
        {
            if (dir == 0) return;
            float height = _container.content.sizeDelta.y;
            float shift = _scrollRate * dir * Time.deltaTime;
            float val = Mathf.Clamp01(_container.verticalNormalizedPosition + shift / height);
            StartCoroutine(ChangeScrollRect(val));
        }

        private void Update() 
        {
            Scroll(_scrollDir);
        }
    }
}
