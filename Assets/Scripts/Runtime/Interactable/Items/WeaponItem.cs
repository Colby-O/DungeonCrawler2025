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

        private BladeController _bc;

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
            return Resources.Load<Sprite>($"Icons/Weapons/{bladeType}_{handleType}_{GetMaterial()}");
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
        }

        public void SetBladeController(BladeController bc)
        {
            _bc = bc;
        }

        public void TakeDurability()
        {
            float amm = DCGameManager.settings.durabilityAmmounts[GetMaterial()];
            amm *= DCGameManager.settings.durabilityRatingScales[GetRating()];
            amm *= DCGameManager.settings.bladeDurabilityScales[this.bladeType];
            _durability -= amm;

            if (_bc != null && !_bc.IsDamged() && _durability < 0.5f)
            {
                _bc.SetDamaged();
                if (GameManager.GetMonoSystem<IInventoryMonoSystem>().GetHandSlot(SlotType.Left).Item == this)
                {
                    BladeController bc = DCGameManager.PlayerController.Sword().GetBladeController();
                    bc.SetDamaged();
                }
            }

            if (_durability <= 0)
            {
                _durability = 0;
            }

            GameManager.GetMonoSystem<IInventoryMonoSystem>().GetHandSlot(SlotType.Left).Refresh();
        }

        protected override float RotationOffset()
        {
            return 90.0f;
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
