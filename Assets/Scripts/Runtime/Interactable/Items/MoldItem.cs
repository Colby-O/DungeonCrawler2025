using UnityEngine;

namespace DC2025
{
    public class MoldItem : PickupableItem
    {
        public BladeType bladeType;
        
        public override Sprite GetIcon()
        {
            Debug.Log($"Icons/Molds/{bladeType}");
            return Resources.Load<Sprite>($"Icons/Molds/{bladeType}"); ;
        }

        public override string GetDescription() => "A mold for a blade.";

        public override string GetName() => "Weapon Mold";
    }
}
