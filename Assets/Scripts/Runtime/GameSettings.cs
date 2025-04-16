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

        public SerializableDictionary<MaterialType, Color> materialColors;
    }
}
