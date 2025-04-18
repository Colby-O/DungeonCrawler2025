using PlazmaGames.Core;
using PlazmaGames.Runtime.DataStructures;
using UnityEngine;

namespace DC2025
{
    public enum PotionType
    {
        Health,
        Stamina,
        Strength,
        Foresight
    }


    public class PotionItem : MaterialItem
    {
        [SerializeField] private MeshRenderer _mr;

        private string GetMessageUse()
        {
            PotionType type = DCGameManager.settings.potionMaterialConverter[_type];
            string msg = string.Empty;
            if (type == PotionType.Health)
            {
                msg = $"heals you for {DCGameManager.settings.healthPotionHealAmount} HP.";
            }
            else if (type == PotionType.Stamina)
            {
                msg = $"restores {DCGameManager.settings.staminaPotionAmount} stamina.";
            }
            else if (type == PotionType.Strength)
            {
                msg = $"allows you to deal {DCGameManager.settings.strengthPotionDamageMul}x more damage for {DCGameManager.settings.strengthPotionTime} seconds.";
            }
            else if (type == PotionType.Foresight)
            {
                msg = $"allows you to see incoming attacks {DCGameManager.settings.foresightAmount} seconds earlier for {DCGameManager.settings.foresightPotionTime} seconds.";
            }
            return msg;
        }

        private string GetMessage()
        {
            PotionType type = DCGameManager.settings.potionMaterialConverter[_type];
            string msg = string.Empty;
            if (type == PotionType.Health)
            {
                msg = $"heals {DCGameManager.settings.healthPotionHealAmount} HP.";
            }
            else if (type == PotionType.Stamina)
            {
                msg = $"restores {DCGameManager.settings.staminaPotionAmount} stamina.";
            }
            else if (type == PotionType.Strength)
            {
                msg = $"allows you to deal {DCGameManager.settings.strengthPotionDamageMul}x more damage for {DCGameManager.settings.strengthPotionTime} seconds.";
            }
            else if (type == PotionType.Foresight)
            {
                msg = $"allows you to see incoming attacks {DCGameManager.settings.foresightAmount} seconds earlier for {DCGameManager.settings.foresightPotionTime} seconds.";
            }
            return msg;
        }

        public void Use()
        {
            PotionType type = DCGameManager.settings.potionMaterialConverter[_type];

            if (type == PotionType.Health)
            {
                DCGameManager.PlayerManager.Heal(DCGameManager.settings.healthPotionHealAmount);
            }
            else if (type == PotionType.Stamina)
            {
                DCGameManager.PlayerManager.AddStamina(DCGameManager.settings.staminaPotionAmount);
            }
            else if (type == PotionType.Strength) 
            {
                GameManager.GetMonoSystem<IFightMonoSystem>().AddDamageBoost(DCGameManager.settings.strengthPotionDamageMul, DCGameManager.settings.strengthPotionTime);
            }
            else if (type == PotionType.Foresight)
            {
                GameManager.GetMonoSystem<IFightMonoSystem>().AddForesightBoost(DCGameManager.settings.foresightAmount, DCGameManager.settings.foresightPotionTime);
            }

            GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send($"You use a <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[_type])}>{type}</color> potion which {GetMessageUse()}");
        }

        public override string GetDescription()
        {
            PotionType type = DCGameManager.settings.potionMaterialConverter[_type];

            return $"{type} potion which {GetMessage()}";
        }

        public override Sprite GetIcon()
        {
            return Resources.Load<Sprite>($"Icons/Potions/{GetMaterial()}");
        }

        public override string GetName()
        {
            PotionType type = DCGameManager.settings.potionMaterialConverter[_type];
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[_type])}>{type} Potion</color>";
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
            _mr.materials[1].color = DCGameManager.settings.materialColors[_type];
        }
    }
}
