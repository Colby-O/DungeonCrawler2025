using System;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace DC2025
{
    public class HandleIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public HandleType _handle = HandleType.Balanced;
        public void OnPointerEnter(PointerEventData eventData)
        {
            CusorPopup p = GameManager.GetMonoSystem<IInventoryMonoSystem>().GetPopup();
            p.Enable();
            string name = $"{_handle} Handle";
            string desc = _handle switch
            {
                HandleType.Balanced => "A balanced handle with equal trade-offs.",
                HandleType.Dominant => "Deals the most damage but costs more stamina with a slow swing speed.",
                HandleType.Lightweight => "Faster Swing speed but deals less damage.",
                HandleType.Rugged => "Deals more damage costing more stamina.",
                HandleType.Wise => "Gives you better foresight into enemy attacks. Has a weaker attack.",
            };
            p.SetText($"<align=center>{name}</align>\n\n<align=left>{desc}</align>");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CusorPopup p = GameManager.GetMonoSystem<IInventoryMonoSystem>().GetPopup();
            p.Disable();
        }
    }
}
