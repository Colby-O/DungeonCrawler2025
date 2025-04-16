using PlazmaGames.Core.MonoSystem;
using UnityEngine;

namespace DC2025
{
    public interface IInventoryMonoSystem : IMonoSystem
    {
        public MouseItem GetMouseSlot();
        public CusorPopup GetPopup();
        public bool AddItemToInventory(PickupableItem item);
        public void RefreshInventory();
        public InventorySlot GetHandSlot(SlotType slot);
    }
}
