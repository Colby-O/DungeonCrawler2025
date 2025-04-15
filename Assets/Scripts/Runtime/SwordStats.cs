using UnityEngine;

namespace DC2025
{
    public class SwordStats
    {
        public float damage = 12;
        public float speed = 0.25f;
        public float foresight = 0;
        public float staminaMultiplier = 1;

        public int ApplyStamina(int stamina) => Mathf.FloorToInt(stamina * this.staminaMultiplier);
    }
}
