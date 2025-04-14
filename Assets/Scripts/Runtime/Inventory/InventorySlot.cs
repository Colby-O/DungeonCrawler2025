using DC2025.Utils;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DC2025
{
    public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI")]
        [SerializeField] private Image _icon;

        [Header("Infomaton")]
        [SerializeField] SlotType _type;

        private IInventoryMonoSystem _inventory;

        public PickupableItem Item { get; set; }

        public bool HasItem() => Item != null;

        public void UpdateSlot(PickupableItem obj)
        {
            Item = obj;
            _icon.sprite = Item.GetItemData().Icon;
            _icon.color = Color.white;
        }

        public void Clear()
        {
            Item = null;
            _icon.sprite = null;
            _icon.color = Color.clear;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_inventory.GetMouseSlot().HasItem()) 
            {
                if (Item == null)
                {
                    UpdateSlot(_inventory.GetMouseSlot().Item);
                    Debug.Log(Item);
                    _inventory.GetMouseSlot().Clear();
                }
                else
                {
                    PickupableItem temp = Item;
                    Clear();
                    UpdateSlot(_inventory.GetMouseSlot().Item);
                    _inventory.GetMouseSlot().Clear();
                    _inventory.GetMouseSlot().UpdateSlot(temp);
                }

                _inventory.GetPopup().Enable();
                _inventory.GetPopup().SetText(Item.GetItemData().ToString());
            }
            else if (Item != null)
            {
                _inventory.GetPopup().Disable();
                _inventory.GetMouseSlot().UpdateSlot(Item);
                Clear();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_inventory.GetMouseSlot().HasItem() && Item != null)
            {
                _inventory.GetPopup().Enable();
                _inventory.GetPopup().SetText(Item.GetItemData().ToString());
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inventory.GetPopup().Disable();
        }

        private void Awake()
        {
            _inventory = GameManager.GetMonoSystem<IInventoryMonoSystem>();
            Clear();
        }
    }
}
