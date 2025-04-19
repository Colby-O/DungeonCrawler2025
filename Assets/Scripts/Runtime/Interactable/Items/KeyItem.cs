using UnityEngine;

namespace DC2025
{
    public class KeyItem : MaterialItem
    {
        [SerializeField] private MeshRenderer _mr;

        public override string GetDescription()
        {
            return $"A key made of {GetMaterial()}, use to unlock doors.";
        }

        public override Sprite GetIcon()
        {
            return Resources.Load<Sprite>($"Icons/Keys/{GetMaterial()}"); ;
        }

        public override string GetName()
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[GetMaterial()])}>{GetMaterial()} Key</color>";
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
            _mr.material.color = DCGameManager.settings.materialColors[_type];
        }
    }
}
