using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DC2025
{
    public class HelpIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string _helpTitle = "Help?";
        [SerializeField] private string _helpText = "How to help...";
        public void OnPointerEnter(PointerEventData eventData)
        {
            CusorPopup p = GameManager.GetMonoSystem<IInventoryMonoSystem>().GetPopup();
            p.Enable();
            p.SetText($"<align=center>{_helpTitle}</align>\n\n<align=left>{_helpText}</align>");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CusorPopup p = GameManager.GetMonoSystem<IInventoryMonoSystem>().GetPopup();
            p.Disable();
        }
    }
}
