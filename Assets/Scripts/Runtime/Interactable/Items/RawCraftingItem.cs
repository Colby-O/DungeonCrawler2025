using UnityEngine;

namespace DC2025
{
    public class RawCraftingItem : MaterialItem
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

        public override void SetMaterial(MaterialType material)
        {

        }
    }
}
