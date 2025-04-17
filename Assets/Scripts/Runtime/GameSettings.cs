using PlazmaGames.Runtime.DataStructures;
using UnityEngine;

namespace DC2025
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [ColorUsage(true, true)] public Color enemyVisionHighlightColor;
        public int playerAttackStamina = 10;
        public int playerAttackFailStamina = 20;
        public int playerBlockStamina = 10;
        public int playerBlockFailStamina = 35;
        public float enemyDistractedWaitTime = 5.0f;
        public float molderTempDropRate = 0.25f;
        public float molderTimeToWin = 1.00f;
        public float moldMoveSpeed = 2.0f;
        public float forgeCookTime = 4;
        public float anvilHammerShowHitTime = 1.0f;
        public int anvilHammerCount = 4;

        public SerializableDictionary<MaterialType, Color> materialColors;

        [Header("Potion Settings")]
        public SerializableDictionary<MaterialType, PotionType> potionMaterialConverter;
        public int healthPotionHealAmount = 100;
        public int staminaPotionAmount = 100;
        public float strengthPotionDamageMul = 2;
        public float strengthPotionTime = 20f;
        public float foresightAmount = 0.2f;
        public float foresightPotionTime = 20f;
    }
}
