using System;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public enum ItemDataType
    {
        None,
        Default,
        RawCrafting,
        Bucket,
        Mold,
        UnfBlade,
        Blade,
        Weapon,
        Potion,
        Key
    }

    [System.Serializable]
    public class SlotData
    {
        public ItemDataType Type;
        public MaterialType MaterialT;
        public BladeType BladeT;
        public HandleType HandleT;
        public int Rating;
        public float Durability;


        [NonSerialized] private PickupableItem _item;

        public PickupableItem Item { 
            get 
            {
                return _item;
            }
            set 
            { 
                _item = value;
                if (_item != null)
                {
                    Type = FetchItemType();
                }
                else
                {
                    Type = ItemDataType.None;
                }
            }
        }

        public void LoadSlot()
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "Material", MaterialT },
                { "Rating", Rating },
                { "Durability", Durability },
                { "Blade", BladeT },
                { "Handle", HandleT }
            };

            _item = PickupableItem.InstantiateItemOfType(Type, data);
        }

        private ItemDataType FetchItemType()
        {
            ItemDataType type;
            if (_item is RawCraftingItem)
            {
                MaterialT = (_item as RawCraftingItem).GetMaterial();
                type = ItemDataType.RawCrafting;
            }
            else if (_item is BucketItem)
            {
                MaterialT = (_item as BucketItem).GetMaterial();
                Rating = (_item as BucketItem).GetRating();
                type = ItemDataType.Bucket;
            }
            else if (_item is MoldItem)
            {
                BladeT = (_item as MoldItem).bladeType;
                type = ItemDataType.Mold;
            }
            else if (_item is UnfBladeItem)
            {
                MaterialT = (_item as UnfBladeItem).GetMaterial();
                BladeT = (_item as UnfBladeItem).bladeType;
                Rating = (_item as UnfBladeItem).GetRating();
                type = ItemDataType.UnfBlade;
            }
            else if (_item is BladeItem)
            {
                MaterialT = (_item as BladeItem).GetMaterial();
                BladeT = (_item as BladeItem).bladeType;
                Rating = (_item as BladeItem).GetRating();
                type = ItemDataType.Blade;
            }
            else if (_item is WeaponItem)
            {
                MaterialT = (_item as WeaponItem).GetMaterial();
                BladeT = (_item as WeaponItem).bladeType;
                HandleT = (_item as WeaponItem).handleType;
                Rating = (_item as WeaponItem).GetRating();
                Durability = (_item as WeaponItem).GetDurability();
                type = ItemDataType.Weapon;
            }
            else if (_item is PotionItem)
            {
                MaterialT = (_item as PotionItem).GetMaterial();
                type = ItemDataType.Potion;
            }
            else if (_item is KeyItem)
            {
                MaterialT = (_item as KeyItem).GetMaterial();
                type = ItemDataType.Key;
            }
            else
            {
                type = ItemDataType.Default;
            }

            return type;
        }

        private void SaveRequiredData()
        {

        }
    }
}
