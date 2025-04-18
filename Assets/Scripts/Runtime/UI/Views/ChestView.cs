using PlazmaGames.Core;
using PlazmaGames.Core.Debugging;
using PlazmaGames.UI;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public class ChestView : View
    {
        [SerializeField] private List<InventorySlot> _slots;

        private Chest _currentChest = null;

        public void SetCurremtChest(Chest chest) => _currentChest = chest;

        public void SetSlots(List<SlotData> data)
        {
            if (_slots == null) return;
            if (data.Count != _slots.Count) PlazmaDebug.LogWarning("Chest and UI slot count don't match.", "ChestView");

            for (int i = 0; i < Mathf.Min(_slots.Count, data.Count); i++)
            {
                if (data[i].Item != null) _slots[i].UpdateSlot(data[i]);
                else _slots[i].Clear();
            }
        }

        public void FetchSlots(ref List<SlotData> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                data[i].Item = _slots[i].Data.Item;
            }
        }

        public override void Init()
        {

        }

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = true;
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().ToggleInventory(true, false);
        }

        public override void Hide()
        {
            base.Hide();
            DCGameManager.IsPaused = false;
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().ToggleInventory(false, true);
            if (_currentChest != null && _currentChest.IsEnabled) _currentChest.Interact();
        }
    }
}
