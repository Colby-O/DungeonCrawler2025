using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DC2025
{
    public enum MaterialType
    {
        Iron,
        Steel,
        Cobolt,
        Bronze
    }

    public abstract class MaterialItem : PickupableItem
    {
        [Header("Mateiral Settings")]
        [SerializeField] private MaterialType _type;
        [SerializeField] private int _rating = 0;

        public MaterialType GetMaterial() => _type;

        public int GetRating()
        {
            return Mathf.Clamp( _rating, 0, 4);
        }

        public void SetRating(int rating)
        {
            _rating = Mathf.Clamp(rating, 0, 4);
        }

        public abstract void SetMaterial(MaterialType material);
    }
}
