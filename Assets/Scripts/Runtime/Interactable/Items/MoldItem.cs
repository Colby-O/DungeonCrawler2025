using UnityEngine;

namespace DC2025
{
    public class MoldItem : PickupableItem
    {
        public BladeType bladeType;
        
        public override Sprite GetIcon()
        {
            return null;
        }

        public override string GetDescription() => "A mold for a blade.";

        public override string GetName() => "Weapon Mold";
    }
}
