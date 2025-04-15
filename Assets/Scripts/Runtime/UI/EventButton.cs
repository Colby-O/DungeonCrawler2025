using PlazmaGames.Attribute;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DC2025
{
    public class EventButton : Button, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent onPointerUp = new UnityEvent();
        public UnityEvent onPointerDown = new UnityEvent();

        [SerializeField, ReadOnly] private bool _isDisabled = false;

        public bool IsDisabled { 
            get 
            { 
                return _isDisabled;
            } 
            set
            {
                _isDisabled = value;
                targetGraphic.color = _isDisabled ? colors.disabledColor : colors.normalColor;
            }
        }

        public bool IsPointerUsed { get; set; }

        public void ForceHighlightedt(bool state)
        {
            if (!IsPointerUsed) targetGraphic.color = state ? colors.pressedColor : colors.normalColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (IsDisabled) return;

            IsPointerUsed = false;
            base.OnPointerUp(eventData);
            onPointerUp.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsDisabled) return;

            IsPointerUsed = true;
            base.OnPointerDown(eventData);
            onPointerDown.Invoke();

        }
    }
}
