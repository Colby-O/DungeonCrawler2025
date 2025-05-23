using PlazmaGames.Runtime.DataStructures;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DC2025
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [ColorUsage(true, true)] public Color enemyVisionHighlightColor;
        [ColorUsage(true, true)] public Color distractionHighlightColor;
        public int playerAttackStamina = 10;
        public int playerAttackFailStamina = 20;
        public int playerBlockStamina = 10;
        public int playerBlockFailStamina = 35;
        public float enemyDistractedWaitTime = 5.0f;
        public float enemyAttackPlayerNoSwordTime = 1;
        public float molderTempDropRate = 0.25f;
        public float molderTimeToWin = 1.00f;
        public float moldMoveSpeed = 2.0f;
        public float forgeCookTime = 4;
        public float anvilHammerShowHitTime = 1.0f;
        public int anvilHammerCount = 4;
        public SerializableDictionary<MaterialType, float> durabilityAmmounts;
        public SerializableDictionary<int, float> durabilityRatingScales;
        public SerializableDictionary<MaterialType, float> materialDamage;
        public SerializableDictionary<int, float> ratingDamageScales;
        public SerializableDictionary<BladeType, float> bladeDurabilityScales;
        public SerializableDictionary<BladeType, float> bladeDamageScales;
        public SerializableDictionary<BladeType, float> bladeSpeeds;
        [FormerlySerializedAs("bladeStaminas")]public SerializableDictionary<BladeType, float> bladeStaminaScales;
        public SerializableDictionary<HandleType, float> handleDamageScales;
        public SerializableDictionary<HandleType, float> handleSpeedScales;
        public SerializableDictionary<HandleType, float> handleStaminaScales;
        public SerializableDictionary<HandleType, float> handleForesights;

        public SerializableDictionary<MaterialType, Color> materialColors;

        [Header("UI")]
        public Texture2D curosrNormal;
        public Texture2D curosrCanClick;

        [Header("Potion Settings")]
        public SerializableDictionary<MaterialType, PotionType> potionMaterialConverter;
        public int healthPotionHealAmount = 100;
        public int staminaPotionAmount = 100;
        public float strengthPotionDamageMul = 2;
        public float strengthPotionTime = 20f;
        public float foresightAmount = 0.2f;
        public float foresightPotionTime = 20f;

        [Header("Audio Settings")]
        public List<AudioClip> entityStepSounds;
        public AudioClip swordSound;
        public AudioClip blockSound;
        public AudioClip openDoorSound;
        public AudioClip closeDoorSound;
        public AudioClip openChestSound;
        public AudioClip closeChestSound;
        public AudioClip fanFireSound;
        public AudioClip dipInWaterSound;
        public AudioClip takeOutOfWaterSound;
        public List<AudioClip> hitSounds;
        public AudioClip craftSound;
        public AudioClip pickupSound;
        public AudioClip dropSound;
        public AudioClip uiClickSound;
        public AudioClip zipperSound;
        public AudioClip potionSound;
        public AudioClip lockedSound;
        public AudioClip unlockedSound;
        public int viewDistance = 10;
    }
}
