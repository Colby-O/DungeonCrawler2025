using UnityEngine;

namespace DC2025
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [ColorUsage(true, true)] public Color enemyVisionHighlightColor;
    }
}
