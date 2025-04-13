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

        public bool IsPointerUsed { get; set; }

        public void ForceHighlightedt(bool state)
        {
            if (!IsPointerUsed) targetGraphic.color = state ? colors.pressedColor : colors.normalColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            IsPointerUsed = false;
            base.OnPointerUp(eventData);
            onPointerUp.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            IsPointerUsed = true;
            base.OnPointerDown(eventData);
            onPointerDown.Invoke();

        }
    }
}
