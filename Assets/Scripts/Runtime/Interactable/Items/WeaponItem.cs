using PlazmaGames.Core;
using UnityEngine;

namespace DC2025
{
    public enum BladeType
    {
        Dagger,
        Axe,
        ShortSword,
        LongSword,
        BattleAxe,
    }

    public enum HandleType
    {
        Balanced=0,
        Dominant,
        Lightweight,
        Rugged,
        Wise,
    }

    public static class HandleTypeExt
    {
        public static int HandleCount() => System.Enum.GetNames(typeof(HandleType)).Length;
    }
    
    public class WeaponItem : MaterialItem
    {
        [SerializeField, Range(0f, 1f)] private float _durability;

        public BladeType bladeType;
        public HandleType handleType;

        public override string GetName()
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[GetMaterial()])}>{GetMaterial()} {handleType} {bladeType}</color>";
        }

        public override string GetDescription()
        {
            return $"A {GetMaterial()} {bladeType} with a {handleType} handle. Used to kill things.";
        }

        public override Sprite GetIcon()
        {
            return null;
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
        }

        public void TakeDurability()
        {
            float amm = DCGameManager.settings.durabilityAmmounts[GetMaterial()];
            amm *= DCGameManager.settings.durabilityRatingScales[GetRating()];
            _durability -= amm;
            if (_durability <= 0)
            {
                _durability = 0;
            }

            GameManager.GetMonoSystem<IInventoryMonoSystem>().GetHandSlot(SlotType.Left).Refresh();
        }

        public float GetDurability()
        {
            return _durability;
        }

        public void SetDurability(float v)
        {
            _durability = v;
        }
    }
}
