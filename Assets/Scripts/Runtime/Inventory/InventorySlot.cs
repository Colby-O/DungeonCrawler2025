using DC2025.Utils;
using PlazmaGames.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace DC2025
{
    public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI")]
        [SerializeField] private Image _icon;
        [SerializeField] private List<GameObject> _stars;
        [SerializeField] private GameObject _durablityContainer;
        [SerializeField] private RectTransform _durablityProgress;
        [SerializeField] private GameObject _cover;

        [Header("Infomaton")]
        [SerializeField] SlotType _type;
        [SerializeField] private bool _disabled = false;
        [SerializeField] private bool _disablePopup = false;

        private IInventoryMonoSystem _inventory;

        public PickupableItem Item { get; set; }

        public UnityEvent OnChange = new UnityEvent();

        public void SetCoverAmount(float amount)
        {
            _cover.transform.localScale = _cover.transform.localScale.SetY(Mathf.Clamp01(amount));
        }

        public void ToogleDisableState(bool state)
        {
            _disabled = state;
            _cover.SetActive(_disabled);
        }

        public bool HasItem() => Item != null;

        public void ResetRating()
        {
            for (int i = 0; i < _stars.Count; i++)
            {
                _stars[i].SetActive(false);
            }
        }

        public void SetStarSlots(int rating)
        {
            ResetRating();
            for (int i = 0; i < Mathf.Min(rating, _stars.Count); i++)
            {
                _stars[i].SetActive(true);
            }
        }

        public void ToggleDurability(bool state)
        {
            _durablityContainer.SetActive(state);
        }

        public void SetDurability(float val)
        {
            _durablityProgress.localScale = _durablityProgress.localScale.SetX(Mathf.Clamp01(val));
        }

        public void Refresh()
        {
            _icon.sprite = Item.GetIcon();
            _icon.color = Color.white;

            if (Item is MaterialItem)
            {
                SetStarSlots((Item as MaterialItem).GetRating());
            }
            else
            {
                ResetRating();
            }

            if (Item is WeaponItem)
            {
                ToggleDurability(true);
                SetDurability((Item as WeaponItem).GetDurability());
            }
            else
            {
                SetDurability(0);
                ToggleDurability(false);
            }
        }

        public void UpdateSlot(PickupableItem obj)
        {
            Item = obj;
            Item.Hide();
            Refresh();
            OnChange.Invoke();
        }

        public void Clear()
        {
            Item = null;
            _icon.sprite = null;
            _icon.color = Color.clear;
            ResetRating();
            SetDurability(0);
            ToggleDurability(false);
            OnChange.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_disabled) return;

            if (_inventory.GetMouseSlot().HasItem()) 
            {
                if (Item == null)
                {
                    UpdateSlot(_inventory.GetMouseSlot().Item);;
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
                if (!_disablePopup)
                {
                    _inventory.GetPopup().Enable();
                    _inventory.GetPopup().SetText(Item.GetDescription());
                }
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
            if (!_disablePopup && !_inventory.GetMouseSlot().HasItem() && Item != null)
            {
                _inventory.GetPopup().Enable();
                _inventory.GetPopup().SetText(Item.GetDescription());
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
            ToogleDisableState(_disabled);
        }
    }
}
