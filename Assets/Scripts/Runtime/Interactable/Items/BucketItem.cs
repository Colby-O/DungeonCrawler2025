using UnityEngine;

namespace DC2025
{
    public class BucketItem : MaterialItem
    {
        [SerializeField] private MeshRenderer _mr;

        public override string GetName()
        {
            return string.Empty;
        }

        public override string GetDescription()
        {
            return string.Empty;
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
