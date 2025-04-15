using UnityEngine;

namespace DC2025
{
    public class UseableItem : PickupableItem
    {
        [SerializeField] private ItemData data;

        public override string GetName()
        {
            return data.Name;
        }

        public override string GetDescription()
        {
            return data.Description;
        }

        public override Sprite GetIcon()
        {
            return data.Icon;
        }
    }
}
