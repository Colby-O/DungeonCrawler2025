using UnityEngine;

namespace DC2025
{
    public enum BladeType
    {
        Sword,
        Axe,
    }

    public enum HandleType
    {
        Balanced,
        Quick,
        Lightweight,
    }
    
    public class WeaponItem : MaterialItem
    {
        [SerializeField, Range(0f, 1f)] private float _durability;

        public BladeType bladeType;
        public HandleType handleType;

        public override string GetName()
        {
            return string.Empty;
        }

        public override string GetDescription()
        {
            return string.Empty;
        }

        public override Sprite GetIcon()
        {
            return null;
        }

        public override void SetMaterial(MaterialType material)
        {

        }

        public float GetDurability()
        {
            return _durability;
        }
    }
}
