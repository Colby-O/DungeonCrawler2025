using UnityEngine;

namespace DC2025
{
    public class RawCraftingItem : MaterialItem
    {
        [SerializeField] private MeshRenderer _mr;

        public override string GetName()
        {
            return $"{GetMaterial()} Rock";
        }

        public override string GetDescription()
        {
            return $"A rock of {GetMaterial()}, used for crafting weapons. Needs to be smelted in a furnace.";
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
