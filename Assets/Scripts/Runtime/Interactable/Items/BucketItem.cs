using UnityEngine;

namespace DC2025
{
    public class BucketItem : MaterialItem
    {
        [SerializeField] private MeshRenderer _mr;

        public override string GetName()
        {
            return $"Molten {GetMaterial()}";
        }

        public override string GetDescription()
        {
            return $"A bucket of molten {GetMaterial()}. Can be poured into a mold to make a blade.";
        }

        public override Sprite GetIcon()
        {
            return Resources.Load<Sprite>($"Icons/Buckets/{GetMaterial()}"); ;
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
            _mr.materials[1].SetColor("_BaseColor", DCGameManager.settings.materialColors[_type]);
        }
    }
}
