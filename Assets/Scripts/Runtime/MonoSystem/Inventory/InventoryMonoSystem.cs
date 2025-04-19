using PlazmaGames.Attribute;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace DC2025
{
    public enum SlotType
    {
        Main,
        Left,
        Right
    }

    public class InventoryMonoSystem : MonoBehaviour, IInventoryMonoSystem
    {
        [Header("Debugging")]
        [SerializeField, ReadOnly] private CusorPopup _popup;
        [SerializeField, ReadOnly] private MouseItem _mouseSlot;
        [SerializeField, ReadOnly] private List<InventorySlot> _slots;
        [SerializeField, ReadOnly] private InventorySlot _rightSlot;
        [SerializeField, ReadOnly] private InventorySlot _leftSlot;

        public MouseItem GetMouseSlot() => _mouseSlot;

        public CusorPopup GetPopup() => _popup;

        public InventorySlot GetHandSlot(SlotType slot) => (slot == SlotType.Left) ? _leftSlot : _rightSlot;

        public bool HasKeyOfType(MaterialType type)
        {
            List<InventorySlot> slotsToCheck = new List<InventorySlot>(_slots)
            {
                _leftSlot,
                _rightSlot
            };

            foreach (InventorySlot slot in slotsToCheck)
            {
                Debug.Log(slot);
                if (slot.Item != null && slot.Item is KeyItem)
                {
                    if ((slot.Item as KeyItem).GetMaterial() == type) return true;
                }
            }

            return false;
        }

        public bool AddItemToInventory(PickupableItem item)
        {
            bool foundFlag = false;

            List<InventorySlot> slotsToCheck = new List<InventorySlot>(_slots);
            slotsToCheck.Reverse();
            slotsToCheck.Add(_leftSlot);
            slotsToCheck.Add(_rightSlot);

            foreach (InventorySlot slot in slotsToCheck)
            {
                if (!slot.HasItem())
                {
                    foundFlag = true;
                    slot.UpdateSlot(item);
                    item.Hide();
                    break;
                }
            }

            return foundFlag;
        }

        public void RefreshInventory()
        {
            List<InventorySlot> slotsToCheck = new List<InventorySlot>(_slots)
            {
                _leftSlot,
                _rightSlot
            };
            foreach (InventorySlot slot in slotsToCheck)
            {
                slot.Refresh();
            }
        }

        private void Start()
        {
            _mouseSlot = GameObject.FindWithTag("MouseSlot").GetComponent<MouseItem>();
            _popup = FindAnyObjectByType<CusorPopup>();
            _popup.Disable();

            _slots = new List<InventorySlot>();
            GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot");
            foreach (GameObject slot in slots)
            {
                _slots.Add(slot.GetComponent<InventorySlot>());
            }

            _leftSlot = GameObject.FindWithTag("LeftSlot").GetComponent<InventorySlot>();
            _rightSlot = GameObject.FindWithTag("RightSlot").GetComponent<InventorySlot>();
        }
    }
}
