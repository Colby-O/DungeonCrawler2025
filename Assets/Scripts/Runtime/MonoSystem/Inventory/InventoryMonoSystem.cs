using UnityEngine;

namespace DC2025
{
    public class InventoryMonoSystem : MonoBehaviour, IInventoryMonoSystem
    {
        private MouseItem _mouseSlot;

        public MouseItem GetMouseSlot() => _mouseSlot;

        private void Start()
        {
            _mouseSlot = GameObject.FindWithTag("MouseSlot").GetComponent<MouseItem>();
        }
    }
}
