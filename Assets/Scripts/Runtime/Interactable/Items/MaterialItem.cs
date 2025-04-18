using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DC2025
{
    public enum MaterialType
    {
        Bronze=0,   
        Iron,
        Steel,
        Cobalt
    }

    public static class MaterialTypeExt
    {
        public static int Rank(this MaterialType type) => (int)type;
    }

    public abstract class MaterialItem : PickupableItem
    {
        [Header("Mateiral Settings")]
        [SerializeField] protected MaterialType _type;
        [SerializeField] protected int _rating = 0;

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

        protected virtual void Awake()
        {
            SetMaterial(_type);
        }
    }
}
