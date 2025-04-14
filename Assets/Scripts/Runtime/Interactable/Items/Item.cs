using PlazmaGames.Attribute;
using PlazmaGames.SO;
using UnityEngine;

namespace DC2025
{
    public enum ItemType
    {
        Useable,
        Weapon,
        Crafting
    }

    public abstract class Item : BaseSO
    {
        [Header("Infomation")]
        public string Name;
        public string Description;
        [ReadOnly] ItemType Type;

        [Header("Inventory")]
        public Sprite Icon;
    }
}
