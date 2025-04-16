using UnityEngine;

namespace DC2025
{
    public class RawCraftingItem : MaterialItem
    {
        [SerializeField] private ItemData data;
        [SerializeField] private MeshRenderer _mr;

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
            return Resources.Load<Sprite>($"Icons/Rocks/{GetMaterial()}");
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
            _mr.materials[0].color = DCGameManager.settings.materialColors[_type];
        }
    }
}
