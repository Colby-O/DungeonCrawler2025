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

    [CreateAssetMenu(fileName = "Defaultitem", menuName = "Item")]
    public class ItemData : BaseSO
    {
        [Header("Infomation")]
        public string Name;
        public string Description;
        public Sprite Icon;

        public override string ToString()
        {
            return $"<align=center>{Name}</align>\n" +
                   $"<align=left>{Description}</align>\n";
        }
    }
}
