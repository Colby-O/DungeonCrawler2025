using DC2025.Utils;
using PlazmaGames.Animation;
using PlazmaGames.Core;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DC2025
{
    public class CusorPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text _text;
        [SerializeField] private CursorFollow _popup;

        [Header("Tween Settings")]
        [SerializeField] private float _popupTime;
        [SerializeField] private float _targetScale = -1;

        private void PopupStep(float t, float startScale, float endScale)
        {
            _popup.transform.localScale = new Vector3(
                Mathf.Lerp(startScale, endScale, t),
                Mathf.Lerp(startScale, endScale, t),
                Mathf.Lerp(startScale, endScale, t)
            );
        }

        public void Enable()
        {
            _popup.gameObject.SetActive(true);
            _popup.transform.localScale = _targetScale * 1.05f * Vector3.one;
            _popup.PositionRelativeToEdges();
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _popupTime,
                (float t) => PopupStep(t, 0, _targetScale)
            );
        }

        public void Disable()
        {
            _popup.gameObject.SetActive(false);
        }

        public void SetText(string val)
        {
            _text.text = val;
        }

        private void Awake()
        {
            if (_targetScale < 0)
            {
                _targetScale = _popup.transform.localScale.x;
            }
        }

        private void Update()
        {

        }
    }
}
