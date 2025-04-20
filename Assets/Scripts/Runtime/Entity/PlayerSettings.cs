using UnityEngine;

namespace DC2025
{
    [CreateAssetMenu(fileName = "DefaultPlayerSettings", menuName = "Player/Settings")]
    public class PlayerSettings : ScriptableObject
    {
        public float moveSpeed;
        public float turnSpeed;

        public float moveSpeedSetting = 0.3f;
        public float turnSpeedSetting = 0.15f;
    }
}
