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

        public void Use()
        {
            PotionType type = DCGameManager.settings.potionMaterialConverter[_type];

            Debug.Log($"Using potion of type <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[_type])}>{_type}</color> as {type}.");

            string msg = string.Empty;

            if (type == PotionType.Health)
            {
                msg = $"heals you for {DCGameManager.settings.healthPotionHealAmount} HP.";
                DCGameManager.PlayerManager.Heal(DCGameManager.settings.healthPotionHealAmount);
            }
            else if (type == PotionType.Stamina)
            {
                msg = $"restores {DCGameManager.settings.staminaPotionAmount} stamina.";
                DCGameManager.PlayerManager.AddStamina(DCGameManager.settings.staminaPotionAmount);
            }
            else if (type == PotionType.Strength) 
            {
                msg = $"allow you to deal {DCGameManager.settings.strengthPotionDamageMul}x more damage for {DCGameManager.settings.strengthPotionTime} seconds.";
                GameManager.GetMonoSystem<IFightMonoSystem>().AddDamageBoost(DCGameManager.settings.strengthPotionDamageMul, DCGameManager.settings.strengthPotionTime);
            }
            else if (type == PotionType.Foresight)
            {
                msg = $"allow you to see incoming attacks {DCGameManager.settings.foresightAmount} seconds earlier for {DCGameManager.settings.foresightPotionTime} seconds.";
                GameManager.GetMonoSystem<IFightMonoSystem>().AddForesightBoost(DCGameManager.settings.foresightAmount, DCGameManager.settings.foresightPotionTime);
            }

            GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send($"You use a <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[_type])}>{type}</color> potion which {msg}");
        }

        public override string GetDescription()
        {
            return string.Empty;
        }

        public override Sprite GetIcon()
        {
            return Resources.Load<Sprite>($"Icons/Potions/{GetMaterial()}");
        }

        public override string GetName()
        {
            return string.Empty;
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
            _mr.materials[1].color = DCGameManager.settings.materialColors[_type];
        }
    }
}
