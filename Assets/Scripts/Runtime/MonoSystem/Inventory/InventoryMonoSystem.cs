using PlazmaGames.Attribute;
using PlazmaGames.DataPersistence;
using System.Collections.Generic;
using UnityEngine;

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

        //[SerializeField, ReadOnly] private List<SlotData> _slotsData = new List<SlotData>();
        //[SerializeField, ReadOnly] private SlotData _leftSlotData;
        //[SerializeField, ReadOnly] private SlotData _rightSlotData;
        //[SerializeField, ReadOnly] private bool _afterStart = false;

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

        private void InitSlots()
        {
            _leftSlot = GameObject.FindWithTag("LeftSlot").GetComponent<InventorySlot>();
            _rightSlot = GameObject.FindWithTag("RightSlot").GetComponent<InventorySlot>();

            _slots = new List<InventorySlot>();
            GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot");
            for (int i = 0; i < slots.Length; i++)
            {
                InventorySlot invSlot = slots[i].GetComponent<InventorySlot>();
                _slots.Add(invSlot);
            }
        }

        private void Start()
        {
            _mouseSlot = GameObject.FindWithTag("MouseSlot").GetComponent<MouseItem>();
            _popup = FindAnyObjectByType<CusorPopup>();
            _popup.Disable();

            InitSlots();
        }

        //public bool SaveData<TData>(ref TData rawData) where TData : GameData
        //{
        //    DCGameData data = rawData as DCGameData;

        //    List<SlotData> slotDatas = new List<SlotData>();

        //    foreach (InventorySlot invSlot in  _slots)
        //    {
        //        slotDatas.Add(invSlot.Data);
        //    }

        //    data.playerSlots = slotDatas;
        //    data.leftSlot = _leftSlot.Data;
        //    data.rightSlot = _rightSlot.Data;

        //    return true;
        //}

        //public bool LoadData<TData>(TData rawData) where TData : GameData
        //{
        //    DCGameData data = rawData as DCGameData;

        //    _slotsData = new List<SlotData>(data.playerSlots);
        //    _leftSlotData = data.leftSlot;
        //    _rightSlotData = data.rightSlot;

        //    if (_afterStart)
        //    {
        //        InitSlots();
        //    }

        //    return true;
        //}
    }
}
