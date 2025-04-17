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

            Debug.Log($"Using potion of type {_type} as {type}.");

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
