using PlazmaGames.Attribute;
using PlazmaGames.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

namespace DC2025
{
    public class Chest : MonoBehaviour
    {
        [Header("Chest Settings")]
        [SerializeField] private List<PickupableItem> _itemsPrefab;
        [SerializeField] private int _numSlots = 8;

        [SerializeField, ReadOnly] private List<SlotData> _slots;

        public List<SlotData> GetSlots()
        {
            return _slots;
        }

        private void Awake()
        {
            _slots = new List<SlotData>();
            for (int i = 0; i < _numSlots; i++)
            {
                _slots.Add(new SlotData());
            }

            List<SlotData> slotsRandomOrder = _slots.Shuffle();

            for (int i = 0; i < Mathf.Min(_itemsPrefab.Count, _numSlots); i++)
            {
                PickupableItem item = Instantiate(_itemsPrefab[i]);
                item.ForceInit();
                item.Hide();
               slotsRandomOrder[i].Item = item;
            }
        }
    }
}
