using UnityEngine;

namespace DC2025
{
    public abstract class Potion : MonoBehaviour
    {
        public abstract void Use(PlayerManager player);
    }
}
